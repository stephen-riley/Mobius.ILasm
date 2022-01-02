//
// Mono.ILASM.BaseGenericTypeRef
//
// Author(s):
//  Ankit Jain  <jankit@novell.com>
//
// Copyright 2006 Novell, Inc (http://www.novell.com)
//

using Mobius.ILasm.interfaces;
using System.Collections;
using System.Collections.Generic;

namespace Mono.ILASM
{

    public abstract class BaseGenericTypeRef : BaseClassRef
    {
        public BaseGenericTypeRef(ILogger logger, string full_name, bool is_valuetype, ArrayList conv_list, string sig_mod, Dictionary<string, string> errors)
                : base(full_name, is_valuetype, conv_list, sig_mod, logger, errors)
        {
            this.logger = logger;
        }

        /* Used to resolve any gen params in arguments, constraints etc */
        public abstract void Resolve(GenericParameters type_gen_params, GenericParameters method_gen_params);

        /* Only resolves, does not add it to the TypeSpec
           table */
        public abstract void ResolveNoTypeSpec(CodeGen code_gen);

        public override GenericTypeInst GetGenericTypeInst(GenericArguments gen_args)
        {
            logger.Error("Invalid attempt to create '" + FullName + "''" + gen_args + "'");
            errors[nameof(BaseGenericTypeRef)] = $"Invalid attempt to create '{FullName}' '{gen_args}'";
            return null;
        }

        public override PEAPI.Type ResolveInstance(CodeGen code_gen, GenericArguments gen_args)
        {
            logger.Error("Invalid attempt to create '" + FullName + "''" + gen_args+ "'");
            errors[nameof(BaseGenericTypeRef)] = $"Invalid attempt to create '{FullName}' '{gen_args}'";
            return null;
        }
    }


}
