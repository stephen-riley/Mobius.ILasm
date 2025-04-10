//
// Mono.ILASM.SentinelTypeRef
//
// Author(s):
//  Jackson Harper (Jackson@LatitudeGeo.com)
//
// (C) 2003 Jackson Harper, All rights reserved
//

using Mobius.ILasm.interfaces;
using System.Collections;
using System.Collections.Generic;

namespace Mono.ILASM
{

    public class SentinelTypeRef : BaseTypeRef
    {

        public SentinelTypeRef(ILogger logger, Dictionary<string, string> errors) : this(null, null, logger, errors)
        {
        }

        public SentinelTypeRef(ArrayList conv_list, string sig_mod, ILogger logger, Dictionary<string, string> errors)
                : base("...", conv_list, sig_mod, logger, errors)
        {
        }

        public override BaseTypeRef Clone()
        {
            return new SentinelTypeRef((ArrayList)ConversionList.Clone(), sig_mod, logger, errors);
        }

        public override void Resolve(CodeGen code_gen)
        {
            if (is_resolved)
                return;

            type = new PEAPI.Sentinel();
            type = Modify(code_gen, type);

            is_resolved = true;
        }

        protected override BaseMethodRef CreateMethodRef(BaseTypeRef ret_type, PEAPI.CallConv call_conv,
                        string name, BaseTypeRef[] param, int gen_param_count)
        {
            return new TypeSpecMethodRef(this, call_conv, ret_type, name, param, gen_param_count, logger, errors);
        }

        protected override IFieldRef CreateFieldRef(BaseTypeRef ret_type, string name)
        {
            return new TypeSpecFieldRef(this, ret_type, name);
        }

        public override string ToString()
        {
            return "Sentinel  " + full_name;
        }
    }

}

