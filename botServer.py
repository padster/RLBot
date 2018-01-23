import struct
import socket
import sys

class Agent:
    # Had to pick something... ¯\_(ツ)_/¯
    PORT = 3451

    # Shape of single game state packet to send over network. Must match packBytes
    packer = struct.Struct('<iiffffffiiifffiii')

    # First two ints (8 bytes) of packet are alternating 1s and 0s
    PAD_INT = 0b1010101010101010101010101010101

    # Create server bot, start listening on the required port.
    def __init__(self, name, team, index):
        self.index = index # Needed
        print ("Opening non-blocking server socket on %s:%d, packet size %d" % (
            socket.gethostbyname(socket.gethostname()), self.PORT, self.packer.size
        ))
        self.sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.sock.setblocking(0) # never block while checking .accept()
        self.sock.bind(("0.0.0.0", self.PORT))
        self.sock.listen(1) # maximum one connection
        self.client = None

    # Update step, pack relevant game state into a buffer and send over the network.
    def get_output_vector(self, game_tick_packet):
        if self.checkConnection():
            self.client.sendall(self.packBytes(game_tick_packet))
        return [0.0, 0.0, 0.0, 0.0, 0.0, 0, 0, 0] # Stay still! Eventually should drive into goal.

    # Return whether we already have someone listening, or someone is joining right now.
    def checkConnection(self):
        if self.client is not None:
            return True
        try:
            # Establish (non-blocking) connection to client if they're here.
            (self.client, self.clientAddress) = self.sock.accept()
            print (">>>> Connected to %s" % (self.clientAddress))
            return True
        except IOError as e:
            return False

    # Convert game state into byte array to send over network.
    # Dump whatever values you need in here.
    def packBytes(self, gameTickPacket):
        ball = gameTickPacket.gameball.Location
        p0At = gameTickPacket.gamecars[0].Location
        p0Rt = gameTickPacket.gamecars[0].Rotation
        p1At = gameTickPacket.gamecars[1].Location
        p1Rt = gameTickPacket.gamecars[1].Rotation
        return self.packer.pack(
            self.PAD_INT, self.PAD_INT, 
            ball.X, ball.Y, ball.Z,
            p0At.X, p0At.Y, p0At.Z,
            p0Rt.Pitch, p0Rt.Yaw, p0Rt.Roll,
            p1At.X, p1At.Y, p1At.Z,
            p1Rt.Pitch, p1Rt.Yaw, p1Rt.Roll,
        )
