//
// Mono.ILASM.GenericMethodRef
//
// Author(s):
//  Jackson Harper (jackson@ximian.com)
//
// (C) 2003 Ximian, Inc (http://www.ximian.com)
//


using Mobius.ILasm.interfaces;
using System.Collections.Generic;

namespace Mono.ILASM
{

    public class GenericMethodRef : BaseMethodRef
    {

        readonly private BaseMethodRef meth;
        readonly private GenericMethodSig sig;

        public GenericMethodRef(BaseMethodRef meth, GenericMethodSig sig, ILogger logger, Dictionary<string, string> errors)
                : base(null, meth.CallConv, null, "", null, 0, logger, errors)
        {
            this.meth = meth;
            this.sig = sig;
            is_resolved = false;
        }

        public override PEAPI.CallConv CallConv
        {
            get { return meth.CallConv; }
            set { meth.CallConv = value; }
        }

        public override void Resolve(CodeGen code_gen)
        {
            if (is_resolved)
                return;

            meth.Resolve(code_gen);
            peapi_method = code_gen.PEFile.AddMethodSpec(meth.PeapiMethod, sig.Resolve(code_gen));

            is_resolved = true;
        }
    }

}

