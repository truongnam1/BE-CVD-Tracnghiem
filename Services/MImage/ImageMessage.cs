using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tracnghiem.Services.MImage
{
    public class ImageMessage
    {
        public enum Information
        {

        }

        public enum Warning
        {

        }

        public enum Error
        {
            IdNotExisted,
            ObjectUsed,
            CodeHasSpecialCharacter,
            CodeExisted,
            NameEmpty,
            NameOverLength,
            UrlEmpty,
            UrlOverLength,
        }
    }
}
