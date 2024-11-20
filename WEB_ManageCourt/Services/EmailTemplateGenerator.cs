namespace WEB_ManageCourt.Services
{
    public class EmailTemplateGenerator
    {
        public string GenerateBookingConfirmationEmail(
           string customerName,
           string bookingDate,
           string timeSlot,
           decimal totalPrice,
           string customerEmail,
           string customerPhone,
           string notes,
            String paymentMethod
            )
        {
            var method = paymentMethod.Equals("Online") ? "Thanh toán online" : "Thanh toán tại sân";
            string htmlTemplate =
                "<!DOCTYPE html>" +
                "<html lang=\"en\">" +
                "<head>" +
                "<meta charset=\"UTF-8\" />" +
                "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" />" +
                "<title>Xác nhận đặt sân cầu lông</title>" +
                "<style>" +
                "body { font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }" +
                ".email-container { max-width: 600px; margin: 20px auto; background-color: #ffffff; border-radius: 16px; box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15); overflow: hidden; border: 1px solid #e0e0e0; }" +
                ".header { background-color: #004080; color: #ffffff; text-align: center; padding: 20px 0; border-bottom: 2px solid #004080; }" +
                ".header h1 { margin: 0; font-size: 24px; }" +
                ".content { padding: 30px; text-align: center; }" +
                ".content p { font-size: 16px; line-height: 1.6; color: #333; }" +
                ".booking-info { display: block; margin: 15px 0; font-size: 18px; font-weight: bold; color: #004080; background-color: #ffffff; padding: 12px 24px; border-radius: 8px; border: 1px solid #c8e6c9; }" +
                ".footer { background-color: #f9f9f9; padding: 20px; text-align: center; font-size: 14px; color: #555; border-top: 1px solid #e0e0e0; }" +
                ".footer a { color: #004080; text-decoration: none; font-weight: bold; }" +
                "</style>" +
                "</head>" +
                "<body>" +
                "<div class=\"email-container\">" +
                "<div class=\"header\">" +
                "<h1>Xác nhận đặt sân cầu lông</h1>" +
                "<p>Công ty OneDO, Địa chỉ: TP.Cần Thơ</p>" +
                "</div>" +
                "<div class=\"content\">" +
                "<p>Xin chào " + customerName + ",</p>" +
                "<p>Bạn đã đặt sân cầu lông thành công vào ngày " + bookingDate + ".</p>" +
                "<div class=\"booking-info\">Khung giờ: " + timeSlot + "</div>" +
                "<div class=\"booking-info\">Tổng chi phí: " + totalPrice.ToString("N0") + " VND</div>" +
                "<div class=\"booking-info\">Email: " + customerEmail + "</div>" +
                "<div class=\"booking-info\">Số điện thoại: " + customerPhone + "</div>" +
                "<div class=\"booking-info\">Ghi chú: " + notes + "</div>" +
                "<div class=\"booking-info\">Phương thức thanh toán: " + method + "</div>" +
                "<p>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!</p>" +
                "</div>" +
                "<div class=\"footer\">" +
                "<p>Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email này.</p>" +
                "<p>Trân trọng,</p>" +
                "<p>Đội ngũ Hỗ trợ Đặt sân Cầu lông</p>" +
                "</div>" +
                "</div>" +
                "</body>" +
                "</html>";

            return htmlTemplate;
        }

        public string GenerateOtpConfirmationEmail(string customerName, string otpCode)
        {
            string htmlTemplate =
                "<!DOCTYPE html>" +
                "<html lang=\"en\">" +
                "<head>" +
                "<meta charset=\"UTF-8\" />" +
                "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" />" +
                "<title>OTP Confirmation Email</title>" +
                "<style>" +
                "body { font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }" +
                ".email-container { max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 8px; box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1); overflow: hidden; }" +
                ".header { background-color: #004080; color: #ffffff; text-align: center; padding: 20px 0; }" +
                ".header h1 { margin: 0; }" +
                ".content { padding: 20px; text-align: center; }" +
                ".content p { font-size: 16px; line-height: 1.5; color: #333; }" +
                ".otp-code { display: inline-block; margin: 20px 0; font-size: 24px; font-weight: bold; color: #004080; background-color: #f0f9f4; padding: 10px 20px; border-radius: 5px; }" +
                ".footer { background-color: #f4f4f4; padding: 10px; text-align: center; font-size: 14px; color: #777; }" +
                ".footer a { color: #004080; text-decoration: none; }" +
                "</style>" +
                "</head>" +
                "<body>" +
                "<div class=\"email-container\">" +
                "<div class=\"header\">" +
                "<h1>Xác nhận mã OTP</h1>" +
                "</div>" +
                "<div class=\"content\">" +
                "<p>Xin chào " + customerName + ",</p>" +
                "<p>Vui lòng xác nhận email của bạn để hoàn tất quá trình. Nhập mã OTP dưới đây vào trang xác nhận:</p>" +
                "<div class=\"otp-code\">" + otpCode + "</div>" +
                "<p>Mã này có hiệu lực trong 1 phút.</p>" +
                "</div>" +
                "<div class=\"footer\">" +
                "<p>Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email này.</p>" +
                "<p>Trân trọng,</p>" +
                "<p>Đội ngũ Hỗ trợ Đặt sân Cầu lông</p>" +
                "</div>" +
                "</div>" +
                "</body>" +
                "</html>";

            return htmlTemplate;
        }
    }
}
