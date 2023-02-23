﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MqttClient.Core.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class EnumResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal EnumResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("MqttClient.Core.Resources.EnumResources", typeof(EnumResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to image/jpeg.
        /// </summary>
        internal static string ImageJpeg {
            get {
                return ResourceManager.GetString("ImageJpeg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to image/png.
        /// </summary>
        internal static string ImagePng {
            get {
                return ResourceManager.GetString("ImagePng", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CharacterData.
        /// </summary>
        internal static string MqttPayloadFormatIndicator_CharacterData {
            get {
                return ResourceManager.GetString("MqttPayloadFormatIndicator.CharacterData", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unspecified.
        /// </summary>
        internal static string MqttPayloadFormatIndicator_Unspecified {
            get {
                return ResourceManager.GetString("MqttPayloadFormatIndicator.Unspecified", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unknown.
        /// </summary>
        internal static string MqttProtocolVersion_Unknown {
            get {
                return ResourceManager.GetString("MqttProtocolVersion.Unknown", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to V310.
        /// </summary>
        internal static string MqttProtocolVersion_V310 {
            get {
                return ResourceManager.GetString("MqttProtocolVersion.V310", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to V311.
        /// </summary>
        internal static string MqttProtocolVersion_V311 {
            get {
                return ResourceManager.GetString("MqttProtocolVersion.V311", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to V500.
        /// </summary>
        internal static string MqttProtocolVersion_V500 {
            get {
                return ResourceManager.GetString("MqttProtocolVersion.V500", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to QoS 0.
        /// </summary>
        internal static string MqttQualityOfServiceLevel_AtLeastOnce {
            get {
                return ResourceManager.GetString("MqttQualityOfServiceLevel.AtLeastOnce", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to QoS 1.
        /// </summary>
        internal static string MqttQualityOfServiceLevel_AtMostOnce {
            get {
                return ResourceManager.GetString("MqttQualityOfServiceLevel.AtMostOnce", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to QoS 2.
        /// </summary>
        internal static string MqttQualityOfServiceLevel_ExactlyOnce {
            get {
                return ResourceManager.GetString("MqttQualityOfServiceLevel.ExactlyOnce", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DoNotSendOnSubscribe.
        /// </summary>
        internal static string MqttRetainHandling_DoNotSendOnSubscribe {
            get {
                return ResourceManager.GetString("MqttRetainHandling.DoNotSendOnSubscribe", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SendAtSubscribe.
        /// </summary>
        internal static string MqttRetainHandling_SendAtSubscribe {
            get {
                return ResourceManager.GetString("MqttRetainHandling.SendAtSubscribe", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SendAtSubscribeIfNewSubscriptionOnly.
        /// </summary>
        internal static string MqttRetainHandling_SendAtSubscribeIfNewSubscriptionOnly {
            get {
                return ResourceManager.GetString("MqttRetainHandling.SendAtSubscribeIfNewSubscriptionOnly", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to text/plain.
        /// </summary>
        internal static string PlainText {
            get {
                return ResourceManager.GetString("PlainText", resourceCulture);
            }
        }
    }
}
