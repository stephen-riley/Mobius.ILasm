//
// Mono.ILASM.FinallyBlock
//
// Author(s):
//  Jackson Harper (Jackson@LatitudeGeo.com)
//
// (C) 2003 Jackson Harper, All rights reserved
//



namespace Mono.ILASM {

        public class FinallyBlock : ISehClause {

                private HandlerBlock handler_block;

                public FinallyBlock ()
                {

                }

                public void SetHandlerBlock (HandlerBlock hb)
                {
                        handler_block = hb;
                }

                public PEAPI.HandlerBlock Resolve (CodeGen code_gen, MethodDef method)
                {
                        PEAPI.CILLabel from = handler_block.GetFromLabel (code_gen, method);
                        PEAPI.CILLabel to = handler_block.GetToLabel (code_gen, method);
                        PEAPI.Finally phinally = new PEAPI.Finally (from, to);

                        return phinally;
                }
        }

}


