﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// Этот исходный код был создан с помощью xsd, версия=4.8.3928.0.
// 
namespace CrackHash.Models {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://ccfit.nsu.ru/schema/crack-hash-response")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://ccfit.nsu.ru/schema/crack-hash-response", IsNullable=false)]
    public partial class CrackHashWorkerResponse {
        
        private string requestIdField;
        
        private string[] resultsField;
        
        /// <remarks/>
        public string RequestId {
            get {
                return this.requestIdField;
            }
            set {
                this.requestIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Results")]
        public string[] Results {
            get {
                return this.resultsField;
            }
            set {
                this.resultsField = value;
            }
        }
    }
}
