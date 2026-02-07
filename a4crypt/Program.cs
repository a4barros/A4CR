// See https://aka.ms/new-console-template for more information
using a4crypt;


Core.Encrypt("G:\\Documents\\drone.png", "G:\\Documents\\drone.a4cr", "123");
Core.Decrypt("G:\\Documents\\drone.a4cr", "G:\\Documents\\a.png", "123");
