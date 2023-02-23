using MqttClient.Modules.ModulePublisher.Resources;
using MqttCommon;
using MqttHelpers.Attributes;
using MqttHelpers.Converters;
using System.ComponentModel;

namespace MqttClient.Modules.ModulePublisher.Enums
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum ContentTypeEnum
    {
        [Description(MimeTypes.TextPlain)]
        PlainText = 0,
        [Description(MimeTypes.ImageJpeg)]
        ImageJpeg,
        [Description(MimeTypes.ImagePng)]
        ImagePng
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum ContentTypeLocalizableEnum
    {
        [LocalizedDescription("PlainText", typeof(EnumResources))]
        PlainText,
        [LocalizedDescription("ImageJpeg", typeof(EnumResources))]
        ImageJpeg,
        [LocalizedDescription("ImagePng", typeof(EnumResources))]
        ImagePng
    }
}
