#encoding:utf-8

import random
import socket, threading
import pickle
whether_debug = False
# 现在这个整体的逻辑没有什么问题了，多线程之间是共享变量的。
# 现在出现一个问题是比如有一边的线程挂了，但是其他线程还在，依旧没法转发.所以不能让我的服务器挂掉
#经典的网络server设计都是有一个线程只负责接受，然后一个线程负责单独去应用
#所谓的listener不过是开了一个线程，然后这个线程专门负责等待然后处理消息
class wait_200_thread(threading.Thread):
    def __init__(self, server_ob):
        threading.Thread.__init__(self)
        self.parent_server = server_ob
    
    def run(self):
        while True:
            data = self.parent_server.conn.recv(2048)#阻塞的      
            data_utf = data.decode()
            if "Prepare" in data_utf:
                if "client_status:200" in data_utf:
                    self.parent_server.wait_status = True
                    return

class load_config_thread(threading.Thread):
    def __init__(self, server_ob):
        threading.Thread.__init__(self)
        self.parent_server = server_ob
    
    def run(self):
        print(f"run:{str(self.parent_server.ID)}")
        self.parent_server.conn.send((f"Prepare|type:BattleFound,ID:{str(self.parent_server.ID)}" +",seed:"+str(self.parent_server.random_seed) + "$").encode())
        self.parent_server.wait_200() # 两边都会阻塞住,而且这个就是原本的
        print(f"BattleFound Success,ID:{str(self.parent_server.ID)}")
        assert len(self.parent_server.the_op_player) >= 1
        if self.parent_server.whether_send_toself is True:
            self.parent_server.conn.send((self.parent_server.process_load_characters(self.parent_server.characters_package,self.parent_server.ID)).encode())
        for op_object,op_conn,op_address in self.parent_server.the_op_player:
            print(f"opponent:{op_object.ID},{op_address}:start send LoadCharacters")
            if whether_debug is True:
                file2 = open("sample.pkl","rb")
                str1 = pickle.load(file2) 
                print(str1)
                self.parent_server.conn.send(self.parent_server.process_load_characters(str1,1).encode())
                file2.close()
            else:
                self.parent_server.conn.send((self.parent_server.process_load_characters(op_object.characters_package,op_object.ID)).encode())
        self.parent_server.load_config_success = True
        print(f"LoadCharacters Success,ID:{str(self.parent_server.ID)}")
    
    
class server(threading.Thread):
    def __init__(self, conn, address, players:list, sock, wait_match:list):
        threading.Thread.__init__(self)
        self.conn = conn
        self.address = address
        self.players = players
        self.sock = sock
        self.wait_match = wait_match
        self.the_op_player = [] # 暂时用来存储对手的信息
        self.characters_package = None
        self.load_config_success = False
        self.whether_send_toself = True # 是否转发给自己
        self.server_status_direct_send = False  # 用来调整一些服务器的全局信息
        self.random_seed = 0
        self.ID = -1
        self.wait_status = False
        self.go_out = False
        self.load_config_success = False # 代表一个最终的成功

        
    def process_load_characters(self,characters_package_,ID_):
        return characters_package_[:-1] + ",ID:"+ str(ID_) +",type:LoadCharacters$"
    
    def wait_200(self):
        wait_200_thread(self).start()
        while True:
            if self.wait_status is True and self.the_op_player[0][0].wait_status is True:
                self.wait_status = False
                self.the_op_player[0][0].wait_status = False
                self.the_op_player[0][0].go_out = True
                return True
            if self.go_out is True: # 能让另外一个线程也出去
                self.go_out = False
                return True
                    
    
    
        
        
    def init_game(self):
        self.conn.send("Prepare|server_status:200,type:Identify$".encode())
        while True:
            data = self.conn.recv(2048)
            data_utf = data.decode()
            if "Prepare" in data_utf  and ("characters" in data_utf):
                self.characters_package = data_utf
                if whether_debug is True:
                    file1 = open("character_package.pkl","wb")
                    pickle.dump(self.characters_package,file1)
                break
        self.wait_match.append((self,self.conn,self.address)) # 添加到匹配队列里面
        if len(self.wait_match) == 1:
            print("Wait another player")
        
        elif len(self.wait_match) == 2:
                self.wait_match[0][0].the_op_player.append(self.wait_match[1]) # 互相把对方添加进对战列表
                self.wait_match[1][0].the_op_player.append(self.wait_match[0])
                self.wait_match[0][0].random_seed = random.randint(0,1000000)
                self.wait_match[1][0].random_seed =  self.wait_match[0][0].random_seed
                self.wait_match[0][0].ID = 1
                self.wait_match[1][0].ID = 2
                print(f"seed:{self.wait_match[0][0].random_seed}")
                print("successfully match")
                load_config_thread(self.wait_match[0][0]).start()
                load_config_thread(self.wait_match[1][0]).start() # 这2句话同一个进程执行,另一个进程会直接进入run的主函数等待
                self.wait_match.clear()
        else:
            print("Can't have 3 layer") # 这里很明显没有并发性
        return
            
                
                
                

    def run(self):  
        self.init_game()
        while self.load_config_success is False:
            continue
        print(f"load_config_success,ID:{self.ID}")
        self.wait_200() # 这个其实是在等loadCharacter成功，也就是StartBattle的最后确认
        self.server_status_direct_send = True
        self.conn.send("Prepare|type:StartBattle$".encode())
        print(f"StartBattle Success,ID:{self.ID}")
        while True:
            data = self.conn.recv(2048)      
            data_utf = data.decode()
            if self.server_status_direct_send is False:
                if "Prepare" in data_utf:
                    if "client_status:200" in data_utf:
                        self.server_status_direct_send = True
                        self.conn.send("Prepare|type:StartBattle$".encode()) # 这2行代码应该没用了

            else:
                if self.whether_send_toself is True:
                    self.conn.send(data)
                    
                for player in self.the_op_player:
                    try:
                        curr_conn = player[1]  # 这里可能需要重连,现在的问题是这个触发之后整个线程就结束了
                        curr_conn.send(data)
                    except:
                        print(player, " end connection")
                        try:
                            #self.players.remove(player)
                            self.the_op_player.remove(player)
                        except:
                            print("ValueError: list.remove(x): x not in list")
            
            

            data = None

# 只要不重连就全程一个tcp连接
def practice():
    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    sock.bind(('10.0.12.13', 12345))
    sock.listen(5)
    players = []
    wait_match = []
    while True:
        conn, address = sock.accept()
        if address not in players:
            players.append((conn,address))
        server(conn, address, players, sock, wait_match).start()


if __name__ == "__main__":
    try:
        practice()
    except KeyboardInterrupt:
        pass

