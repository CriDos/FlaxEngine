// Copyright (c) 2012-2019 Wojciech Figat. All rights reserved.

using System;
using System.Collections.Generic;

namespace Flax.Build.Bindings
{
    /// <summary>
    /// The native structure information for bindings generator.
    /// </summary>
    public class StructureInfo : ApiTypeInfo
    {
        public AccessLevel Access;
        public TypeInfo BaseType;
        public List<FieldInfo> Fields;
        public List<FunctionInfo> Functions;
        public bool IsAutoSerialization;
        public bool ForceNoPod;

        public override bool IsStruct => true;
        public override bool IsValueType => true;
        public override bool IsPod => _isPod;

        private bool _isPod;

        public override void Init(Builder.BuildData buildData)
        {
            base.Init(buildData);

            if (ForceNoPod)
            {
                _isPod = false;
                return;
            }

            // Structure is POD (plain old data) only if all of it's fields are (and has no base type ro base type is also POD)
            _isPod = BaseType == null || (BindingsGenerator.FindApiTypeInfo(buildData, BaseType, Parent)?.IsPod ?? false);
            for (int i = 0; _isPod && i < Fields.Count; i++)
            {
                var field = Fields[i];
                if (!field.IsStatic && !field.Type.IsPod(buildData, this))
                {
                    _isPod = false;
                }
            }
        }

        public override void AddChild(ApiTypeInfo apiTypeInfo)
        {
            if (apiTypeInfo is EnumInfo)
            {
                base.AddChild(apiTypeInfo);
            }
            else
            {
                throw new NotSupportedException("Structures in API can have only enums as sub-types.");
            }
        }

        public override string ToString()
        {
            return "struct " + Name;
        }
    }
}
