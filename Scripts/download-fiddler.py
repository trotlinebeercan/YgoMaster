#!/usr/bin/env python

import os, re, requests, sys

def download_file_from_google_drive(id, destination):
    CHUNK_SIZE = 32*1024
    URL = 'https://docs.google.com/uc?export=download'

    session = requests.Session()
    response = session.get(URL, params = { 'id' : id }, stream = True)

    token = None
    for key, value in response.cookies.items():
        if key.startswith('download_warning'):
            token = value
            break

    if token:
        params = { 'id' : id, 'confirm' : token }
        response = session.get(URL, params = params, stream = True)

    with open(destination, 'wb') as f:
        for chunk in response.iter_content(CHUNK_SIZE):
            if chunk:
                f.write(chunk)

if __name__ == '__main__':
    if len(sys.argv) == 1:
        file_id = "1XF63HGPB37L235dcW0RLIkplm7R5mt7_"
        destination = "./FiddlerSetup.7z"
    elif len(sys.argv) != 3:
        executable = os.path.basename(sys.argv[0])
        print('Usage: [python] %s {DRIVE_FILE_ID_OR_URL} {DESTINATION_FILE_PATH}' % executable)
    else:
        # shareable link or file id
        if sys.argv[1].startswith('https://'):
            try:
                file_id = re.match(r'[?&]/d/([a-zA-Z0-9_-]+)', sys.argv[1]).group(1)
            except:
                file_id = re.search(r'/d/([a-zA-Z0-9_-]+)/view', sys.argv[1]).group(1)
        else:
            file_id = sys.argv[1]

        # destination on disk
        destination = sys.argv[2]

    download_file_from_google_drive(file_id, destination)
