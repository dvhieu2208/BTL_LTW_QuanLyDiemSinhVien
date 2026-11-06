using System;
using System.ComponentModel.DataAnnotations;

namespace QuanLyDiemDaiHoc.Models
{
    public class CauHinhDangKyHocPhan
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime NgayBatDau { get; set; }

        [Required]
        public DateTime NgayKetThuc { get; set; }
    }
}
