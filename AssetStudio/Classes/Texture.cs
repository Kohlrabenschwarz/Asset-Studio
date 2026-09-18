using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssetStudio
{
    public abstract class Texture : NamedObject
    {
        protected Texture(ObjectReader reader) : base(reader)
        {
            if (version[0] > 2017 || (version[0] == 2017 && version[1] >= 3)) //2017.3 and up
            {
                var nodes = reader.serializedType?.m_Type?.m_Nodes;
                if (nodes?.Count == 0) nodes = null;
                if (nodes?.Any(x => x.m_Level == 1 && x.m_Name == "m_ForcedFallbackFormat") ?? version[0] < 6000)
                {
                    var m_ForcedFallbackFormat = reader.ReadInt32();
                }
                if (nodes?.Any(x => x.m_Level == 1 && x.m_Name == "m_DownscaleFallback") ?? version[0] < 6000)
                {
                    var m_DownscaleFallback = reader.ReadBoolean();
                }
                if (nodes?.Any(x => x.m_Level == 1 && x.m_Name == "m_IsAlphaChannelOptional") ??
                    (version[0] > 2020 || (version[0] == 2020 && version[1] >= 2)))
                {
                    var m_IsAlphaChannelOptional = reader.ReadBoolean();
                }
                reader.AlignStream();
            }
        }
    }
}
