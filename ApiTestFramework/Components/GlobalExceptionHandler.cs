using ApiTestFramework.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace ApiTestFramework.Components
{
    public static class GlobalExceptionHandler
    {
        public static void Handle(Exception ex)
        {
            if (ex is BusinessException businessEx)
            {
                // 业务异常（可提示用户）
                ShowBusinessError(businessEx);
            }
            else
            {
                // 系统异常（记录日志 + 友好提示）
                Log(ex);
                ShowSystemError();
            }
        }

        private static void ShowBusinessError(BusinessException ex)
        {
            MessageBox.Show(ex.Message, "业务提示");
        }

        private static void ShowSystemError()
        {
            MessageBox.Show("系统异常，请联系管理员", "错误");
        }

        private static void Log(Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}
