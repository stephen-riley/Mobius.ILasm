//
// Mono.ILASM.GenericParameters
//
// Author(s):
//  Ankit Jain  <jankit@novell.com>
//
// Copyright 2006 Novell, Inc (http://www.novell.com)
//

using Mobius.ILasm.interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Mono.ILASM
{

    public class GenericParameter : ICustomAttrTarget
    {
        readonly string id;
        int num;
        readonly PEAPI.GenericParamAttributes attr;
        ArrayList constraintsList;
        ArrayList customattrList;

        public GenericParameter(string id, ILogger logger)
            : this(id, 0, null, logger)
        {
        }

        public GenericParameter(string id, PEAPI.GenericParamAttributes attr, ArrayList constraints, ILogger logger)
        {
            this.id = id;
            this.attr = attr;
            num = -1;
            constraintsList = null;
            customattrList = null;

            if (constraints != null)
                foreach (BaseTypeRef typeref in constraints)
                    AddConstraint(typeref);
        }

        public string Id
        {
            get { return id; }
        }

        public int Num
        {
            get { return num; }
            set { num = value; }
        }

        public void AddConstraint(BaseTypeRef constraint)
        {
            if (constraint == null)
                throw new InternalErrorException();

            constraintsList ??= [];

            constraintsList.Add(constraint);
        }

        public override string ToString()
        {
            return Id;
        }

        public void AddCustomAttribute(CustomAttr customattr)
        {
            customattrList ??= [];

            customattrList.Add(customattr);
        }

        public void Resolve(CodeGen code_gen, PEAPI.MethodDef methoddef)
        {
            PEAPI.GenericParameter gp = methoddef.AddGenericParameter((short)num, id, attr);
            Resolve(code_gen, gp);
        }

        public void Resolve(CodeGen code_gen, PEAPI.ClassDef classdef)
        {
            PEAPI.GenericParameter gp = classdef.AddGenericParameter((short)num, id, attr);
            Resolve(code_gen, gp);
        }

        private void Resolve(CodeGen code_gen, PEAPI.GenericParameter gp)
        {
            ResolveConstraints(code_gen, gp);
            if (customattrList == null)
                return;

            foreach (CustomAttr customattr in customattrList)
                customattr.AddTo(code_gen, gp);
        }

        public void ResolveConstraints(GenericParameters type_gen_params, GenericParameters method_gen_params)
        {
            if (constraintsList == null)
                return;

            foreach (BaseTypeRef constraint in constraintsList)
            {
                if (constraint is BaseGenericTypeRef gtr)
                    gtr.Resolve(type_gen_params, method_gen_params);
            }
        }

        private void ResolveConstraints(CodeGen code_gen, PEAPI.GenericParameter gp)
        {
            if (constraintsList == null)
                return;

            foreach (BaseTypeRef constraint in constraintsList)
            {
                constraint.Resolve(code_gen);
                gp.AddConstraint(constraint.PeapiType);
            }
        }

    }

    public class GenericParameters
    {
        ArrayList param_list;
        string param_str;
        readonly ILogger logger;
        readonly private Dictionary<string, string> errors;

        public GenericParameters(ILogger logger, Dictionary<string, string> errors)
        {
            param_list = null;
            param_str = null;
            this.logger = logger;
            this.errors = errors;
        }

        public int Count
        {
            get { return (param_list == null ? 0 : param_list.Count); }
        }

        public GenericParameter this[int index]
        {
            get { return (param_list != null ? (GenericParameter)param_list[index] : null); }
            set { Add(value); }
        }

        public void Add(GenericParameter gen_param)
        {
            if (gen_param == null)
                throw new InternalErrorException();

            param_list ??= [];
            gen_param.Num = param_list.Count;
            param_list.Add(gen_param);
            param_str = null;
        }

        public GenericParameter GetGenericParam(string id)
        {
            if (param_list == null)
            {
                logger.Error("Invalid type parameter '" + id + "'");
                errors[nameof(GenericParameters)] = $"Invalid type parameter '{id}'";
            }

            foreach (GenericParameter param in param_list)
                if (param.Id == id)
                    return param;
            return null;
        }

        public int GetGenericParamNum(string id)
        {
            GenericParameter param = GetGenericParam(id);
            if (param != null)
                return param.Num;

            return -1;
        }

        public void Resolve(CodeGen code_gen, PEAPI.ClassDef classdef)
        {
            foreach (GenericParameter param in param_list)
                param.Resolve(code_gen, classdef);
        }

        public void Resolve(CodeGen code_gen, PEAPI.MethodDef methoddef)
        {
            foreach (GenericParameter param in param_list)
                param.Resolve(code_gen, methoddef);
        }

        public void ResolveConstraints(GenericParameters type_gen_params, GenericParameters method_gen_params)
        {
            foreach (GenericParameter param in param_list)
                param.ResolveConstraints(type_gen_params, method_gen_params);
            param_str = null;
        }

        private void MakeString()
        {
            //Build full_name (foo < , >)
            StringBuilder sb = new StringBuilder();
            sb.Append('<');
            foreach (GenericParameter param in param_list)
                sb.AppendFormat("{0}, ", param);
            //Remove the extra ', ' at the end
            sb.Length -= 2;
            sb.Append('>');
            param_str = sb.ToString();
        }

        public override string ToString()
        {
            if (param_str == null)
                MakeString();
            return param_str;
        }
    }

}
