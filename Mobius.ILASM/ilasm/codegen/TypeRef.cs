//
// Mono.ILASM.TypeRef
//
// Author(s):
//  Jackson Harper (Jackson@LatitudeGeo.com)
//
// (C) 2003 Jackson Harper, All rights reserved
//

using Mobius.ILasm.interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Mono.ILASM
{

    /// <summary>
    /// Reference to a type in the module being compiled.
    /// </summary>
    public class TypeRef : BaseClassRef
    {

        readonly private Location location;
        public static readonly TypeRef Ellipsis = new TypeRef(null, "ELLIPSIS", false, null, default);
        public static readonly TypeRef Any = new TypeRef(null, "any", false, null, default);

        public TypeRef(ILogger logger, string full_name, bool is_valuetype, Location location, Dictionary<string, string> errors)
                : this(logger, full_name, is_valuetype, location, null, null, errors)
        {
        }

        public TypeRef(ILogger logger, string full_name, bool is_valuetype, Location location, ArrayList conv_list, string sig_mod, Dictionary<string, string> errors)
                : base(full_name, is_valuetype, conv_list, sig_mod, logger, errors)
        {
            this.location = location;
        }

        public override BaseTypeRef Clone()
        {
            return new TypeRef(logger, full_name, is_valuetype, location, (ArrayList)ConversionList.Clone(), sig_mod, errors);
        }

        protected override BaseMethodRef CreateMethodRef(BaseTypeRef ret_type,
                PEAPI.CallConv call_conv, string name, BaseTypeRef[] param, int gen_param_count)
        {
            if (SigMod == null | SigMod == "")
                return new MethodRef(this, call_conv, ret_type, name, param, gen_param_count, logger, errors);
            else
                return new TypeSpecMethodRef(this, call_conv, ret_type, name, param, gen_param_count, logger, errors);
        }

        protected override IFieldRef CreateFieldRef(BaseTypeRef ret_type, string name)
        {
            return new FieldRef(this, ret_type, name);
        }

        public override void Resolve(CodeGen code_gen)
        {
            if (is_resolved)
                return;

            PEAPI.Type base_type;

            base_type = code_gen.TypeManager.GetPeapiType(full_name);

            if (base_type == null)
            {
                logger?.Error("Reference to undefined class '" +
                                       FullName + "'");
                errors[nameof(TypeRef)] = $"Reference to undefined class {FullName}";
                Debug.Assert(logger != null);
                return;
            }
            type = Modify(code_gen, base_type);

            is_resolved = true;
        }

        public BaseClassRef AsClassRef(CodeGen code_gen)
        {
            return this;
        }

        public override string ToString()
        {
            return FullName;
        }

    }

}

