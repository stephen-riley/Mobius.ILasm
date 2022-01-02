//
// Mono.ILASM.ExternTypeRef
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

    /// <summary>
    /// A reference to a type in another assembly
    /// </summary>
    public class ExternTypeRef : BaseClassRef, IScope
    {

        readonly private IScope extern_ref;
        readonly private Hashtable nestedtypes_table;
        readonly private Hashtable nestedclass_table;

        public ExternTypeRef(IScope extern_ref, ILogger logger, string full_name, bool is_valuetype, Dictionary<string, string> errors)
                : this(extern_ref, logger, full_name, is_valuetype, null, null, errors)
        {
        }

        private ExternTypeRef(IScope extern_ref, ILogger logger, string full_name,
                        bool is_valuetype, ArrayList conv_list, string sig_mod, Dictionary<string, string> errors)
    : base(full_name, is_valuetype, conv_list, sig_mod, logger, errors)
        {
            this.extern_ref = extern_ref;

            nestedclass_table = new Hashtable();
            nestedtypes_table = new Hashtable();
        }

        public override BaseTypeRef Clone()
        {
            return new ExternTypeRef(extern_ref, logger, full_name, is_valuetype,
                            (ArrayList)ConversionList.Clone(), sig_mod, errors);
        }

        public override string FullName
        {
            get
            {
                if (extern_ref == null)
                    return full_name + sig_mod;
                else
                    return extern_ref.FullName + (extern_ref is ExternTypeRef ? "/" : "") + full_name + sig_mod;
            }
        }

        public string Name
        {
            get { return full_name + sig_mod; }
        }

        public IScope ExternRef
        {
            get { return extern_ref; }
        }

        public override void Resolve(CodeGen code_gen)
        {
            if (is_resolved)
                return;

            if (extern_ref is ExternTypeRef etr)
                //This is a nested class, so resolve parent
                etr.Resolve(code_gen);

            type = extern_ref.GetType(full_name, is_valuetype);
            type = Modify(code_gen, type);

            is_resolved = true;
        }

        protected override BaseMethodRef CreateMethodRef(BaseTypeRef ret_type, PEAPI.CallConv call_conv,
                        string name, BaseTypeRef[] param, int gen_param_count)
        {
            return new ExternMethodRef(this, ret_type, call_conv, name, param, gen_param_count, logger, errors);
        }

        protected override IFieldRef CreateFieldRef(BaseTypeRef ret_type, string name)
        {
            return new ExternFieldRef(this, ret_type, name);
        }

        public ExternTypeRef GetTypeRef(string _name, bool is_valuetype)
        {
            string first = _name;
            string rest = "";
            int slash = _name.IndexOf('/');

            if (slash > 0)
            {
                first = _name.Substring(0, slash);
                rest = _name.Substring(slash + 1);
            }

            if (nestedtypes_table[first] is ExternTypeRef ext_typeref)
            {
                if (is_valuetype && rest == "")
                    ext_typeref.MakeValueClass();
            }
            else
            {
                ext_typeref = new ExternTypeRef(this, logger, first, is_valuetype, errors);
                nestedtypes_table[first] = ext_typeref;
            }

            return (rest == "" ? ext_typeref : ext_typeref.GetTypeRef(rest, is_valuetype));
        }

        public PEAPI.IExternRef GetExternTypeRef()
        {
            //called by GetType for a nested type
            //should this cant be 'modified' type, so it should
            //be ClassRef 
            return (PEAPI.ClassRef)type;
        }

        public PEAPI.ClassRef GetType(string _name, bool is_valuetype)
        {
            if (nestedclass_table[_name] is PEAPI.ClassRef klass)
                return klass;

            ExternTable.GetNameAndNamespace(_name, out string name_space, out string name);

            if (is_valuetype)
                klass = (PEAPI.ClassRef)GetExternTypeRef().AddValueClass(name_space, name);
            else
                klass = (PEAPI.ClassRef)GetExternTypeRef().AddClass(name_space, name);

            nestedclass_table[_name] = klass;

            return klass;
        }

        public System.Type GetReflectedType()
        {
            if (extern_ref is ExternRef er)
            {
                if (er is ExternAssembly ea)
                {
                    System.Reflection.Assembly asm = System.Reflection.Assembly.Load(ea.Name);

                    //Type name required here, so don't use FullName
                    return asm.GetType(Name);
                }/* else ExternModule */

            } /*else - nested type? */
            return null;
        }
    }

}

