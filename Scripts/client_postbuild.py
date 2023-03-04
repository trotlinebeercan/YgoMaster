#!/usr/bin/env python

# if "$(ConfigurationName)" == "InstallDebug" (
# call $(SolutionDir)\Scripts\client_postbuild.bat $(SolutionDir)
# )

# if "$(ConfigurationName)" == "InstallRelease" (
# call $(SolutionDir)\Scripts\client_postbuild.bat $(SolutionDir)
# )

import argparse, json, os, shutil

SCRIPT_DIR = os.path.realpath(os.path.dirname(__file__))
ALLOWLIST = [
    "YgoMasterClient.exe",
    "YgoMasterLoader.dll"
]

def get_parsed_args():
    def dir_path(string):
        if os.path.isdir(string):
            return string
        else:
            raise NotADirectoryError(string)

    parser = argparse.ArgumentParser(prog="copy_built", description="todo", epilog="bottom text?")
    parser.add_argument("-i", "--source_dir", metavar="SOURCE_DIR", type=dir_path, required=True, help="Input directory")
    parser.add_argument("-o", "--target_dir", metavar="TARGET_DIR", type=dir_path, help="Output directory")
    return parser.parse_args()

def create_directories(target_dir):
    if not os.path.exists(target_dir):
        os.makedirs(target_dir)

def remove(target):
    if os.path.isfile(target):
        os.remove(target)
    if os.path.isdir(target):
        os.rmdir(target)

def remove_files_in_directory(target_dir, delete_directories):
    #print(f"> Deleting all files in {target_dir}")
    for root, dirs, files in os.walk(target_dir, topdown=False):
        for f in files:
            remove(os.path.join(root, f))
        if delete_directories:
            for d in dirs:
                remove(os.path.join(root, d))

def copy_file(source_file, target_file):
    if os.path.isfile(source_file):
        shutil.copy2(source_file, target_file)

def copy_files_to_directory(source_dir, target_dir):
    #print(f"> Starting walk at {source_dir}")
    for root, dirs, files in os.walk(source_dir, topdown=True, followlinks=False):
        for f in files:
            src = os.path.join(root, f)
            rel = os.path.relpath(src, source_dir)
            tgt = os.path.join(target_dir, rel)
            #print(f"====> Copying {src} to {tgt}")
            copy_file(src, tgt)
        for d in dirs:
            src = os.path.join(root, d)
            rel = os.path.relpath(src, source_dir)
            tgt = os.path.join(target_dir, rel)
            #print(f"====> Calling {src} to {tgt}")
            create_directories(tgt)

if __name__ == "__main__":
    args = get_parsed_args()
    src_dir = args.source_dir

    tgt_dir = ""
    if args.target_dir:
        tgt_dir = args.target_dir
    else:
        with open(os.path.join(SCRIPT_DIR, "client_postbuild_paths.json")) as jf:
            data = json.load(jf)
            tgt_dir = data["target_dir"]

    if os.path.isdir(tgt_dir):
        for f in ALLOWLIST:
            remove(os.path.join(tgt_dir, f))
            copy_file(os.path.join(src_dir, f), os.path.join(tgt_dir, f))

    # remove_files_in_directory(tgt_dir, True)
    # copy_files_to_directory(src_dir, tgt_dir)