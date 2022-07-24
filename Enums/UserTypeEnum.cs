using System.Collections.Generic;
using TrueSight.Common;

namespace Tracnghiem.Enums
{
    public class UserTypeEnum
    {
        public static string USER_TYPE_CLAIM_TYPE = "UserType";

        public static GenericEnum APP_USER = new GenericEnum ( Id : 1, Code : "APP_USER", Name : "Người dùng thường" );
        public static GenericEnum STORE_USER = new GenericEnum ( Id : 2, Code : "STORE_USER", Name : "Người dùng đại lý" );
        public static GenericEnum SUPPLIER_USER = new GenericEnum ( Id : 3, Code : "SUPPLIER_USER", Name : "Người dùng NCC" );

        public static List<GenericEnum> UserTypeEnumList = new List<GenericEnum>
        {
            APP_USER, STORE_USER, SUPPLIER_USER,
        };
    }
}
