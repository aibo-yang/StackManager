using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StackManager.Context.Domain
{
    abstract class IEntity
    {
        [Display(Name = "主键")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual Guid Id { get; set; }

        [Display(Name = "行版本")]
        [Timestamp]
        [ConcurrencyCheck]
        public virtual byte[] RowVersion { get; set; }

        [Display(Name = "更新时间")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public virtual DateTime LastUpdated { get; set; }

        [Display(Name = "数据是否删除")]
        public virtual bool SoftDeleted { get; set; } = false;

        public override bool Equals(object obj)
        {
            if (obj is IEntity that)
            {
                if (Equals(Id, Guid.Empty) && Equals(that.Id, Guid.Empty))
                {
                    return ReferenceEquals(this, that);
                }
                return Id.Equals(that.Id);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            if (Equals(Id, Guid.Empty))
            {
                return base.GetHashCode();
            }
            return Id.GetHashCode();
        }

        public static bool operator ==(IEntity left, IEntity right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(IEntity left, IEntity right)
        {
            return !Equals(left, right);
        }
    }
}