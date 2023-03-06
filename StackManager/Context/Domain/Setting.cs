using System.ComponentModel.DataAnnotations;

namespace StackManager.Context.Domain
{
    /// <summary>
    /// 系统配置
    /// </summary>
    class Setting : IEntity
    {
        [Display(Name = "设备名称"), MaxLength(200)]
        public string Name { get; set; }

        [Display(Name = "设备编号"), MaxLength(200)]
        public string SerialNumber { get; set; }

        [Display(Name = "MES地址"), MaxLength(200)]
        public string MesUri { get; set; }

        [Display(Name = "MES密码"), MaxLength(200)]
        public string MesSecret { get; set; }

        [Display(Name = "MES令牌"), MaxLength(200)]
        public string MesTokenId { get; set; }

        [Display(Name = "PQM地址"), MaxLength(200)]
        public string PqmUri { get; set; }

        [Display(Name = "登录密码"), MaxLength(200)]
        public string Password { get; set; }
    }
}
