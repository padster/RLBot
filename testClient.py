"""
Test client for reading network packets. Use for debugging.
"""
import socket
import struct
import sys

# Should match the structure of packer in botServer
unpacker = struct.Struct('<iiffffffiiifffiii')

# Connect to the server...
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_address = (socket.gethostbyname(socket.gethostname()), 3451)
print ('Connecting to %s port %s....' % server_address)
sock.connect(server_address)
print ('Connected!')

# Keep reading and printing, reading and printing, reading and printing, ...
while True:
    try:
        recv = sock.recv(unpacker.size)
        data = unpacker.unpack(recv)
        print ((
            "Border: %d %d\n" +
            "Ball at: (%.2f, %.2f, %.2f)\n" +
            "P0 at: (%.2f, %.2f, %.2f)\n" + 
            "P0 rot: (%d, %d, %d)\n" +
            "P1 at: (%.2f, %.2f, %.2f)\n" +
            "P1 rot: (%d, %d, %d)\n\n"
        ) % data)
    except IOError as e:
        print ("whoops")
        print (e)