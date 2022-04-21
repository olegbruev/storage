﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MtdKey.Storage
{    
    public class NodeSchema
    {
        public long NodeId { get; set; }
        public long BunchId { get; set; }
        public int Number { get; set; }
        public List<NodeSchemaItem> Items { get; set; }
    }
}
