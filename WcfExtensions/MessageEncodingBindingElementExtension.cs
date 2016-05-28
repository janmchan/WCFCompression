/**
 * File name: MessageEncodingBindingElementExtension.cs 
 * Author: Mosfiqur.Rahman
 * Date: 11/12/2009 4:07:51 PM format: MM/dd/yyyy
 * 
 * 
 * Modification history:
 * Name				Date					Desc
 * 
 *  
 * Version: 1.0
 * */

#region Using Directives

using System;
using System.ComponentModel;
using System.Configuration;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;

#endregion

namespace WcfExtensions
{
    /// <summary>
    /// Summary of this class.
    /// </summary>
    public class MessageEncodingBindingElementExtension : BindingElementExtensionElement
    {

        private const string Config_Property_InnerMessageEncoding = "innerMessageEncoding";
        private const string Config_Property_MessageEncoderType = "messageEncoderType";
        private const string Config_Property_MessageVersion = "messageVersion";
        private const string Config_Property_InnerMessageEncoding_DefaultValue = "textMessageEncoding";
        private const string Config_Property_MessageVersion_DefaultValue = "Soap12";
        public const string Config_Property_MessageEncoderType_DefaultValue = "WcfExtensions.GZipMessageEncoder, WcfExtensions";
        private const string Config_Property_MaxArrayLength = "MaxArrayLength";
        private const string Category_ReaderQuotas = "ReaderQuotas";
        private const string Config_Property_ReaderQuotas = "readerQuotas";
        private const string BinaryMessageEncoding = "binaryMessageEncoding";
        private const string MtomMessageEncoding = "mtomMessageEncoding";
        private const string Config_Property_MaxDepth = "maxDepth";
        private const string Config_Property_MaxStringContentLength = "maxStringContentLength";
        private const string Config_Property_MaxBytesPerRead = "maxBytesPerRead";
        private const string Config_Property_MaxNameTableCharCount = "maxNameTableCharCount";


        //The only property we need to configure for our binding element is the type of
        //inner encoder to use. Here, we support text and binary.
        [ConfigurationProperty(Config_Property_InnerMessageEncoding, DefaultValue = Config_Property_InnerMessageEncoding_DefaultValue)]
        public string InnerMessageEncoding
        {
            get { return (string)base[Config_Property_InnerMessageEncoding]; }
            set { base[Config_Property_InnerMessageEncoding] = value; }
        }

        [ConfigurationProperty(Config_Property_MessageEncoderType, DefaultValue = Config_Property_MessageEncoderType_DefaultValue)]
        public string MessageEncoderType
        {
            get
            {
                return (string)base[Config_Property_MessageEncoderType];
            }
            set
            {
                base[Config_Property_MessageEncoderType] = value;
            }
        }
        [ConfigurationProperty(Config_Property_MessageVersion, DefaultValue = Config_Property_MessageVersion_DefaultValue)]
        public MessageVersion MessageVersionType
        {
            get
            {
                return (MessageVersion)base[Config_Property_MessageVersion];
            }
            set
            {
                base[Config_Property_MessageVersion] = value;
            }
        }

        /// <summary>
        /// Gets or sets the length of the max array.
        /// </summary>
        /// <value>The length of the max array.</value>
        [ConfigurationProperty(Config_Property_MaxArrayLength, DefaultValue = 999999999, IsRequired = false)]
        [Category(Category_ReaderQuotas)]
        [Description(Config_Property_MaxArrayLength)]
        public int MaxArrayLength
        {
            get { return ((XmlDictionaryReaderQuotasElement)base[Config_Property_ReaderQuotas]).MaxArrayLength; }
            set { ((XmlDictionaryReaderQuotasElement)base[Config_Property_ReaderQuotas]).MaxArrayLength = value; }
        }


        [ConfigurationProperty(Config_Property_MaxDepth, DefaultValue = 999999999, IsRequired = false)]
        [Category(Category_ReaderQuotas)]
        [Description(Config_Property_MaxDepth)]
        public int MaxDepth
        {
            get
            {
                return ((XmlDictionaryReaderQuotasElement)base[Config_Property_ReaderQuotas]).MaxDepth;
            }
            set
            {
                ((XmlDictionaryReaderQuotasElement)base[Config_Property_ReaderQuotas]).MaxDepth = value;
            }
        }


        [ConfigurationProperty(Config_Property_MaxStringContentLength, DefaultValue = 999999999, IsRequired = false)]
        [Category(Category_ReaderQuotas)]
        [Description(Config_Property_MaxStringContentLength)]
        public int MaxStringContentLength
        {
            get
            {
                return ((XmlDictionaryReaderQuotasElement)base[Config_Property_ReaderQuotas]).MaxStringContentLength;
            }
            set
            {
                ((XmlDictionaryReaderQuotasElement)base[Config_Property_ReaderQuotas]).MaxStringContentLength = value;
            }
        }


        [ConfigurationProperty(Config_Property_MaxBytesPerRead, DefaultValue = 999999999, IsRequired = false)]
        [Category(Category_ReaderQuotas)]
        [Description(Config_Property_MaxBytesPerRead)]
        public int MaxBytesPerRead
        {
            get
            {
                return ((XmlDictionaryReaderQuotasElement)base[Config_Property_ReaderQuotas]).MaxBytesPerRead;
            }
            set
            {
                ((XmlDictionaryReaderQuotasElement)base[Config_Property_ReaderQuotas]).MaxBytesPerRead = value;
            }
        }


        [ConfigurationProperty(Config_Property_MaxNameTableCharCount, DefaultValue = 999999999, IsRequired = false)]
        [Category(Category_ReaderQuotas)]
        [Description(Config_Property_MaxNameTableCharCount)]
        public int MaxNameTableCharCount
        {
            get
            {
                return ((XmlDictionaryReaderQuotasElement)base[Config_Property_ReaderQuotas]).MaxNameTableCharCount;
            }
            set
            {
                ((XmlDictionaryReaderQuotasElement)base[Config_Property_ReaderQuotas]).MaxNameTableCharCount = value;
            }
        }

        /// <summary>
        /// Gets the reader quotas.
        /// </summary>
        /// <value>The reader quotas.</value>
        [ConfigurationProperty(Config_Property_ReaderQuotas)]
        public XmlDictionaryReaderQuotasElement ReaderQuotas
        {
            get { return (XmlDictionaryReaderQuotasElement)base[Config_Property_ReaderQuotas]; }
        }

        //Called by the WCF to discover the type of binding element this config section enables
        public override Type BindingElementType
        {
            get { return typeof(ExtendedMessageEncodingBindingElement); }
        }

        //Called by the WCF to apply the configuration settings (the property above) to the binding element
        public override void ApplyConfiguration(BindingElement bindingElement)
        {
            var binding = (ExtendedMessageEncodingBindingElement)bindingElement;

            PropertyInformationCollection propertyInfo = this.ElementInformation.Properties;
            if (propertyInfo[Config_Property_InnerMessageEncoding].ValueOrigin != PropertyValueOrigin.Default)
            {
                switch (this.InnerMessageEncoding)
                {
                    case Config_Property_InnerMessageEncoding_DefaultValue:
                        binding.InnerMessageEncodingBindingElement = new TextMessageEncodingBindingElement();

                        if (ReaderQuotas.MaxArrayLength > 0)
                        {
                            ((TextMessageEncodingBindingElement)binding.InnerMessageEncodingBindingElement).
                                ReaderQuotas.
                                MaxArrayLength = ReaderQuotas.MaxArrayLength;
                        }
                        if (ReaderQuotas.MaxBytesPerRead > 0)
                        {
                            ((TextMessageEncodingBindingElement)binding.InnerMessageEncodingBindingElement).
                                ReaderQuotas.
                                MaxBytesPerRead = ReaderQuotas.MaxBytesPerRead;
                        }
                        if (ReaderQuotas.MaxDepth > 0)
                        {
                            ((TextMessageEncodingBindingElement)binding.InnerMessageEncodingBindingElement).
                                ReaderQuotas.
                                MaxDepth = ReaderQuotas.MaxDepth;
                        }
                        if (ReaderQuotas.MaxNameTableCharCount > 0)
                        {
                            ((TextMessageEncodingBindingElement)binding.InnerMessageEncodingBindingElement).
                                ReaderQuotas.
                                MaxNameTableCharCount = ReaderQuotas.MaxNameTableCharCount;
                        }
                        if (ReaderQuotas.MaxStringContentLength > 0)
                        {
                            ((TextMessageEncodingBindingElement)binding.InnerMessageEncodingBindingElement).
                                ReaderQuotas.
                                MaxStringContentLength = ReaderQuotas.MaxStringContentLength;
                        }
                        break;
                    case BinaryMessageEncoding:
                        binding.InnerMessageEncodingBindingElement = new BinaryMessageEncodingBindingElement();
                        if (ReaderQuotas.MaxArrayLength > 0)
                        {
                            ((BinaryMessageEncodingBindingElement)binding.InnerMessageEncodingBindingElement).
                                ReaderQuotas.
                                MaxArrayLength = ReaderQuotas.MaxArrayLength;
                        }
                        if (ReaderQuotas.MaxBytesPerRead > 0)
                        {
                            ((BinaryMessageEncodingBindingElement)binding.InnerMessageEncodingBindingElement).
                                ReaderQuotas.
                                MaxBytesPerRead = ReaderQuotas.MaxBytesPerRead;
                        }
                        if (ReaderQuotas.MaxDepth > 0)
                        {
                            ((BinaryMessageEncodingBindingElement)binding.InnerMessageEncodingBindingElement).
                                ReaderQuotas.
                                MaxDepth = ReaderQuotas.MaxDepth;
                        }
                        if (ReaderQuotas.MaxNameTableCharCount > 0)
                        {
                            ((BinaryMessageEncodingBindingElement)binding.InnerMessageEncodingBindingElement).
                                ReaderQuotas.
                                MaxNameTableCharCount = ReaderQuotas.MaxNameTableCharCount;
                        }
                        if (ReaderQuotas.MaxStringContentLength > 0)
                        {
                            ((BinaryMessageEncodingBindingElement)binding.InnerMessageEncodingBindingElement).
                                ReaderQuotas.
                                MaxStringContentLength = ReaderQuotas.MaxStringContentLength;
                        }
                        break;
                    case MtomMessageEncoding:
                        binding.InnerMessageEncodingBindingElement = new MtomMessageEncodingBindingElement();
                        if (ReaderQuotas.MaxArrayLength > 0)
                        {
                            ((MtomMessageEncodingBindingElement)binding.InnerMessageEncodingBindingElement).
                                ReaderQuotas.
                                MaxArrayLength = ReaderQuotas.MaxArrayLength;
                        }
                        if (ReaderQuotas.MaxBytesPerRead > 0)
                        {
                            ((MtomMessageEncodingBindingElement)binding.InnerMessageEncodingBindingElement).
                                ReaderQuotas.
                                MaxBytesPerRead = ReaderQuotas.MaxBytesPerRead;
                        }
                        if (ReaderQuotas.MaxDepth > 0)
                        {
                            ((MtomMessageEncodingBindingElement)binding.InnerMessageEncodingBindingElement).
                                ReaderQuotas.
                                MaxDepth = ReaderQuotas.MaxDepth;
                        }
                        if (ReaderQuotas.MaxNameTableCharCount > 0)
                        {
                            ((MtomMessageEncodingBindingElement)binding.InnerMessageEncodingBindingElement).
                                ReaderQuotas.
                                MaxNameTableCharCount = ReaderQuotas.MaxNameTableCharCount;
                        }
                        if (ReaderQuotas.MaxStringContentLength > 0)
                        {
                            ((MtomMessageEncodingBindingElement)binding.InnerMessageEncodingBindingElement).
                                ReaderQuotas.
                                MaxStringContentLength = ReaderQuotas.MaxStringContentLength;
                        }
                        break;
                }
            }
        }

        //Called by the WCF to create the binding element
        protected override BindingElement CreateBindingElement()
        {
            var bindingElement = new ExtendedMessageEncodingBindingElement(this.MessageEncoderType);
            bindingElement.MessageVersion = this.MessageVersionType;
            ApplyConfiguration(bindingElement);
            return bindingElement;
        }

    }
}

// end of namespace
