﻿using System.Collections.Generic;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    [Title("Input", "Post Process Inputs")]
    public class PostProcessInputsNode : AbstractMaterialNode, IGenerateProperties, IGeneratesBodyCode, IMayRequireScreenPosition
    {
        const string kUVSlotName = "UV";
        const string kOutputSlotName = "Source";

        public const int UVSlotId = 0;
        public const int OutputSlotId = 1;

        public PostProcessInputsNode()
        {
            name = "Post Process Inputs";
            UpdateNodeAfterDeserialization();
        }

        public override bool hasPreview
        {
            get { return false; }
        }

        public override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new ScreenPositionMaterialSlot(UVSlotId, kUVSlotName, kUVSlotName));
            AddSlot(new Vector4MaterialSlot(OutputSlotId, kOutputSlotName, kOutputSlotName, SlotType.Output, Vector4.zero));
            RemoveSlotsNameNotMatching(new[] { UVSlotId, OutputSlotId });
        }

        /*public override void CollectPreviewMaterialProperties(List<PreviewProperty> properties)
        {
            properties.Add(new PreviewProperty(PropertyType.Vector4)
            {
                name = "_MainTex",
                vector4Value = new Vector4(1, 1, 1, 1)
            });
        }

        public override void CollectShaderProperties(PropertyCollector properties, GenerationMode generationMode)
        {
            properties.AddShaderProperty(new TextureShaderProperty
            {
                overrideReferenceName = "_MainTex",
                generatePropertyBlock = true,
				modifiable = false
            });
        }*/

        public void GenerateNodeCode(ShaderGenerator visitor, GenerationMode generationMode)
        {            
			var result = string.Format("{0}4 {1} = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, {2});"
                , precision
				, GetVariableNameForSlot(OutputSlotId)
                , GetSlotValue(UVSlotId, generationMode));

            visitor.AddShaderChunk(result, true);
        }

        public bool RequiresScreenPosition()
        {
            var uvSlot = FindInputSlot<MaterialSlot>(UVSlotId) as ScreenPositionMaterialSlot;
            if (uvSlot == null)
                return false;

            if (uvSlot.isConnected)
                return false;

            return uvSlot.RequiresScreenPosition();
        }
    }
}
