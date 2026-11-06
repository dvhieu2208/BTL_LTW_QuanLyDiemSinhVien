# Nâng cấp giao diện – QuanLyDiemDaiHoc

Phong cách: Modern University Management System (Blue & White, glass nhẹ), font Inter, Bootstrap 5.3.3 (local), Font Awesome 6.4, Chart.js 4.4.1 (chỉ tải ở trang có biểu đồ).

## File mới
| File | Vai trò |
|---|---|
| `wwwroot/css/ums-core.css` | Design system dùng chung: token màu, card, button, form/validation, table, badge, toast, modal, animation, responsive, `prefers-reduced-motion`, View Transitions |
| `wwwroot/js/ums-core.js` | Sidebar thu gọn/off-canvas, Toast (`UMS.toast`), modal xác nhận (`data-confirm-trigger`), loading + chống gửi lặp, tooltip, count-up, lọc + phân trang bảng phía client, hiện/ẩn mật khẩu, Việt hoá thông báo validation |
| `Views/Shared/_FlashMessages.cshtml` | Đọc TempData (`Success`, `Error`, `ThongBao`, …) → Toast (có fallback khi tắt JS) |
| `Areas/Admin/Views/_ViewImports.cshtml`, `Areas/Student/Views/_ViewImports.cshtml` | Tag helper + using chung cho Areas |
| `Areas/Admin/Models/DashboardItems.cs`, `Areas/Student/Models/DiemHocKyItem.cs` | Lớp hiển thị cho Dashboard (không ánh xạ DB) |

## Controller (chỉ bổ sung, chỉ đọc dữ liệu)
- `Admin/HomeController.Index`: thêm thống kê phòng học, chuyên ngành, lịch học, lượt đăng ký, bản ghi điểm, tài khoản hoạt động/bị khóa, SV theo khoa, 6 lượt đăng ký gần nhất, đợt đăng ký.
- `Student/HomeController.Index`: thêm số môn đạt/chưa đạt (quy tắc ≥ 5.0 như trang bảng điểm), ĐTB theo học kỳ, điểm gần đây, số học phần đã đăng ký, đợt đăng ký.
- `Admin/DotDangKyHocPhanController.Index (GET)`: thêm `ViewBag.DaCoCauHinh` để hiển thị đúng trạng thái "chưa thiết lập".

Không thay đổi: Entity, DbContext, database, route, action, tên field form, quy tắc tính điểm/đạt môn, xác thực & phân quyền.

## Sửa lỗi giao diện phát hiện khi rà soát
- Trang Login hiển thị `ViewBag.ThongBao` nhưng Controller trả `ViewBag.Error` → lỗi đăng nhập trước đây không hiện. Đã hiển thị đúng (alert + toast).
- `login-style.css` dùng `@@media` (không hợp lệ trong file .css) → responsive login không chạy. Đã sửa.
- Admin layout nạp `_ValidationScriptsPartial` nhưng không có jQuery → lỗi console. Đã thêm jQuery.
- Thông báo TempData `Error` ở Tài khoản, `Success/Error` ở Hồ sơ sinh viên không hiển thị → nay hiện bằng Toast.
- Sidebar Admin thiếu trạng thái active cho Thời khóa biểu/Tài khoản; header Student không hiện tên SV (dùng Session).
- Bộ lọc khoa (Khoa & Lớp) không giữ lựa chọn sau khi lọc.
- Form sửa sinh viên thiếu trường Chức vụ (Controller đã cung cấp danh sách) → lưu sẽ xoá chức vụ. Đã thêm trường.
- `ViewBag.HighlightSv` (Sửa điểm từ bảng điểm) đã có ở Controller nhưng chưa dùng → nay tô sáng và focus đúng sinh viên.

## Lưu ý
- Phân trang danh sách là phân trang phía client trên dữ liệu đã tải (backend chưa hỗ trợ phân trang).
- "Học kỳ hiện tại" trên dashboard là ước tính theo lịch năm học (hệ thống chưa có bảng cấu hình học kỳ) và được ghi chú rõ trên giao diện.
- Không có bảng Giảng viên / trạng thái "đang học" trong CSDL nên dashboard không hiển thị các số liệu này (không tạo số liệu giả).
