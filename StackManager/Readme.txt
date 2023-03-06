SELECT p.LastUpdated, p.Id,p.`Code`,b.PalletNo, pc.`Name` FROM pallets p
LEFT JOIN boxes b ON p.Id = b.PalletId
LEFT JOIN productcategories pc ON b.ProductCategoryId = pc.Id
WHERE p.LastUpdated > '2022-06-16 08:00:00' && p.`Code` != ''




//新增Password字段
ALTER TABLE Settings ADD COLUMN Password VARCHAR(200) DEFAULT '695F3A335A8181FE226702BE2336A1F1';