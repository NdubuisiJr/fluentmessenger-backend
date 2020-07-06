using System;

namespace FluentMessenger.API.Exceptions {
    public class PatchIdNotFoundException : ArgumentException{
        public PatchIdNotFoundException(string message) : base (message){

        }
    }
}
