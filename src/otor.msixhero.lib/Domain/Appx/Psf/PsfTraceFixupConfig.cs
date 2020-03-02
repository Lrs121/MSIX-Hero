﻿using System.ComponentModel;
using System.Runtime.Serialization;
using Windows.Foundation.Metadata;
using Newtonsoft.Json;

namespace otor.msixhero.lib.Domain.Appx.Psf
{
    public class PsfTraceFixupConfig : PsfFixupConfig
    {
        [DataMember(Name = "traceMethod")]
        [DefaultValue("default")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string TraceMethod { get; set; }
        
        [DataMember(Name = "traceLevels")]
        public PsfTraceFixupLevels TraceLevels { get; set; }
        
        [DataMember(Name = "breakOn")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(TraceLevel.UnexpectedFailures)]
        public TraceLevel BreakOn { get; set; }

        public override string ToString()
        {
            return $"Trace method {this.TraceMethod}, break on {this.BreakOn}";
        }
    }
}