using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nager.AuthenticationService.WebApi.Helpers;

namespace Nager.AuthenticationService.UnitTest
{
    [TestClass]
    public class RoleHelperTest
    {
        [TestMethod]
        public void AddRoleToRoleData_RoleDataNull_Successful()
        {
            var roleData = RoleHelper.AddRoleToRoleData(null, "test");

            Assert.AreEqual(roleData, "test");
        }

        [TestMethod]
        public void AddRoleToRoleData_RoleDataStringEmpty_Successful()
        {
            var roleData = RoleHelper.AddRoleToRoleData(string.Empty, "test");

            Assert.AreEqual(roleData, "test");
        }

        [TestMethod]
        public void AddRoleToRoleData_DuplicateRoleTest_Successful()
        {
            var roleData = RoleHelper.AddRoleToRoleData("test", "test");

            Assert.AreEqual(roleData, "test");
        }

        [TestMethod]
        public void AddRoleToRoleData_DuplicateRoleTestPascalCase_Successful()
        {
            var roleData = RoleHelper.AddRoleToRoleData("test", "Test");

            Assert.AreEqual(roleData, "test");
        }
    }
}
