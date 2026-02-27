using System;
using System.Security.Cryptography;
using System.Text;

namespace MouseClickCounter.Services
{
    /// <summary>
    /// 设备信息服务
    /// </summary>
    public class DeviceInfoService
    {
        /// <summary>
        /// 获取设备信息
        /// </summary>
        public DeviceInfo GetDeviceInfo()
        {
            var deviceInfo = new DeviceInfo();

            try
            {
                // 获取设备名称（计算机名）
                deviceInfo.DeviceName = Environment.MachineName;

                // 生成设备唯一ID（基于计算机名和硬件信息）
                string userName = Environment.UserName;
                string osVersion = Environment.OSVersion.VersionString;
                string combined = $"{deviceInfo.DeviceName}_{userName}_{osVersion}";

                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
                    deviceInfo.DeviceId = BitConverter.ToString(hash).Replace("-", "").Substring(0, 16);
                }

                deviceInfo.UserName = userName;
                deviceInfo.OSVersion = osVersion;
                deviceInfo.IsSuccess = true;
            }
            catch (Exception ex)
            {
                deviceInfo.DeviceName = "Unknown";
                deviceInfo.DeviceId = Guid.NewGuid().ToString().Substring(0, 8);
                deviceInfo.UserName = "Unknown";
                deviceInfo.OSVersion = "Unknown";
                deviceInfo.IsSuccess = false;
                deviceInfo.ErrorMessage = ex.Message;
            }

            return deviceInfo;
        }

        /// <summary>
        /// 设备信息类
        /// </summary>
        public class DeviceInfo
        {
            /// <summary>
            /// 设备ID
            /// </summary>
            public string DeviceId { get; set; } = string.Empty;

            /// <summary>
            /// 设备名称
            /// </summary>
            public string DeviceName { get; set; } = string.Empty;

            /// <summary>
            /// 用户名
            /// </summary>
            public string UserName { get; set; } = string.Empty;

            /// <summary>
            /// 操作系统版本
            /// </summary>
            public string OSVersion { get; set; } = string.Empty;

            /// <summary>
            /// 是否成功获取
            /// </summary>
            public bool IsSuccess { get; set; }

            /// <summary>
            /// 错误信息
            /// </summary>
            public string? ErrorMessage { get; set; }

            /// <summary>
            /// 转换为字符串表示
            /// </summary>
            public override string ToString()
            {
                return $"设备ID: {DeviceId}\r\n" +
                       $"设备名称: {DeviceName}\r\n" +
                       $"用户名: {UserName}\r\n" +
                       $"操作系统: {OSVersion}\r\n" +
                       $"获取成功: {IsSuccess}";
            }
        }
    }
}
