//
// Mono.ILASM.GenericParamRef
//
// Author(s):
//  Jackson Harper (Jackson@LatitudeGeo.com)
//  Ankit Jain  (JAnkit@novell.com)
//
// (C) 2003 Jackson Harper, All rights reserved
// (C) 2006 Novell, Inc (http://www.novell.com)
//


using Mobius.ILasm.interfaces;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Mono.ILASM
{

    public class GenericParamRef : BaseGenericTypeRef
    {

        /* PEAPI.Type type: Might get modified */
        /* Unmodified GenParam */
        readonly private PEAPI.GenParam param;
        private bool is_added; /* Added to TypeSpec table ? */
        readonly private static Hashtable param_table = new Hashtable();

        public GenericParamRef(PEAPI.GenParam gen_param, ILogger logger, string full_name, Dictionary<string, string> errors)
                : this(gen_param, logger, full_name, null, errors)
        {

        }

        public GenericParamRef(PEAPI.GenParam gen_param, ILogger logger, string full_name, ArrayList conv_list, Dictionary<string, string> errors)
                : base(logger, full_name, false, conv_list, "", errors)
        {
            this.type = gen_param;
            this.param = gen_param;
            is_added = false;
        }

        public override string FullName
        {
            get
            {
                return (param.Type == PEAPI.GenParamType.Var ? "!" : "!!")
                        + param.Index + sig_mod;
            }
        }

        public override void MakeValueClass()
        {
            throw new InternalErrorException("Not supported");
        }

        public override BaseTypeRef Clone()
        {
            return new GenericParamRef(param, logger, full_name, (ArrayList)ConversionList.Clone(), errors);
        }

        public override void ResolveNoTypeSpec(CodeGen code_gen)
        {
            if (is_resolved)
                return;

            type = Modify(code_gen, type);
            is_resolved = true;
        }

        public override void Resolve(CodeGen code_gen)
        {
            ResolveNoTypeSpec(code_gen);
            if (is_added)
                return;

            string key = param.Type.ToString() + param.Index.ToString();
            PEAPI.GenParam val = (PEAPI.GenParam)param_table[key];
            if (val == null)
            {
                code_gen.PEFile.AddGenericParam(param);
                param_table[key] = param;
            }
            else
            {
                /* Set this instance's "type" to the cached
                   PEAPI.GenParam, after applying modifications */
                type = Modify(code_gen, val);
            }

            is_added = true;
        }

        public override void Resolve(GenericParameters type_gen_params, GenericParameters method_gen_params)
        {
            if (param.Name == "")
            {
                /* Name wasn't specified */
                return;
            }

            if (param.Type == PEAPI.GenParamType.MVar && method_gen_params != null)
                param.Index = method_gen_params.GetGenericParamNum(param.Name);
            else if (type_gen_params != null)
                param.Index = type_gen_params.GetGenericParamNum(param.Name);

            if (param.Index < 0)
                logger.Error(String.Format("Invalid {0}type parameter '{1}'",
                                        (param.Type == PEAPI.GenParamType.MVar ? "method " : ""),
                                         param.Name));
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
    }

}

