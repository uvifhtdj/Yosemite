import os
import sys
import time
import shutil
from fundrive.drives.baidu.drive import BaiDuDrive
import io

# 设置标准输出的编码为 UTF-8
sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')

print("Python 解释器路径:", sys.executable)
print("环境变量:", os.environ)
print("模块搜索路径:", sys.path)

# 检查是否提供了足够的参数
if len(sys.argv) != 3:
    print("用法: python script.py <本地文件路径> <远程文件路径>")
    sys.exit(1)

local_file_path = sys.argv[1]
remote_file_path = sys.argv[2]

bduss = os.environ.get('BAIDU_BDUSS')

try:
    print("=== 1. 初始化和登录 ===")
    client = BaiDuDrive()
    login_result = client.login(bduss=bduss)
    if login_result:
        print("登录成功!")
    else:
        print("登录失败!")
except Exception as e:
    print(f"初始化失败: {e}")
    import traceback
    traceback.print_exc()
    sys.exit(2)

#for path in client.get_file_list("/其他"):
#    print(path['name'])
upload_result = client.upload_file(local_file_path, remote_file_path)
print(f"上传结果: {upload_result}")
if not upload_result:
    sys.exit(3)