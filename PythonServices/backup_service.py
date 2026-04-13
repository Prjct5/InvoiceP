import argparse
import base64
import requests
import datetime
from config import get_db_connection, output_success, output_error

def main(args):
    try:
        if getattr(args, 'test', False):
            return {"message": "Test successful", "commit": "testsha123456"}
            
        conn = get_db_connection(args.db_path)
        settings = conn.execute("SELECT * FROM store_settings WHERE id = 1").fetchone()
        
        repo = settings['github_repo']
        token = settings['github_token']
        branch = settings['github_branch'] or 'main'
        
        if not repo or not token:
            raise ValueError("GitHub repository or token not configured in store settings")
            
        # Read the db file as base64
        with open(args.db_path, "rb") as f:
            content = f.read()
            encoded_content = base64.b64encode(content).decode('ascii')
            
        timestamp = datetime.datetime.now().strftime("%Y%m%d_%H%M%S")
        filename = f"furnitrack_{timestamp}.db"
        url = f"https://api.github.com/repos/{repo}/contents/backups/{filename}"
        
        headers = {
            "Authorization": f"Bearer {token}",
            "Accept": "application/vnd.github.v3+json"
        }
        
        payload = {
            "message": f"Auto-backup {timestamp}",
            "content": encoded_content,
            "branch": branch
        }
        
        response = requests.put(url, headers=headers, json=payload)
        
        if response.status_code in [200, 201]:
            data = response.json()
            commit_sha = data.get('commit', {}).get('sha', '')
            return {"message": "Backup successful", "commit": commit_sha}
        else:
            raise ValueError(f"GitHub API Error: {response.status_code} - {response.text}")
            
    except Exception as e:
        output_error(str(e))

if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("--db-path", type=str, required=False)
    parser.add_argument("--test", action="store_true")
    args = parser.parse_args()
    
    result = main(args)
    if result:
        output_success(result)
