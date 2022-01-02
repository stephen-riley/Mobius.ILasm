//
// Mono.ILASM.ILTokenizingException
//
// Author(s):
//  Jackson Harper (jackson@ximian.com)
//
// Copyright 2004 Novell, Inc (http://www.novell.com)
//



namespace Mono.ILASM {

        public class ILTokenizingException : Mobius.ILasm.infrastructure.ILAsmException
    {

                public readonly string Token;

                public ILTokenizingException (Location location, string token)
                        : base (location, token)
                {
                        Token = token;
                }
        }

}


