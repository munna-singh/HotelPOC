using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public abstract class BaseModel
    {
        public BaseModel()
        {
            InitializeDefaults();
        }

        public virtual string ToJson()
        {
            var ms = new MemoryStream();
            new DataContractJsonSerializer(GetType()).WriteObject(ms, this);
            ms.Position = 0;
            return new StreamReader(ms).ReadToEnd();
        }

        public virtual string ToXml()
        {
            throw new NotImplementedException();
        }

        public virtual void InitializeDefaults()
        {

        }
    }
}
