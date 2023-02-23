using MqttCommon;
using MqttCore.Resources;
using MqttHelpers.Attributes;
using MqttHelpers.Converters;
using System.ComponentModel;

namespace MqttCore.Enums
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
