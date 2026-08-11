-- Script: enable_on_delete_cascade.sql
-- Purpose: Recreate all foreign key constraints that reference dbo.Stack to include ON DELETE CASCADE.
-- Usage: BACKUP your database before running. Run in context of the 'flashcards' database.

SET NOCOUNT ON;

DECLARE @fkId INT;
DECLARE @fkName SYSNAME;
DECLARE @childSchema SYSNAME;
DECLARE @childTable SYSNAME;
DECLARE @parentSchema SYSNAME;
DECLARE @parentTable SYSNAME;
DECLARE @childCols NVARCHAR(MAX);
DECLARE @parentCols NVARCHAR(MAX);
DECLARE @sql NVARCHAR(MAX);

DECLARE fk_cursor CURSOR FOR
SELECT fk.object_id, fk.name, schc.name AS ChildSchema, tc.name AS ChildTable, shp.name AS ParentSchema, tp.name AS ParentTable
FROM sys.foreign_keys fk
JOIN sys.tables tc ON fk.parent_object_id = tc.object_id
JOIN sys.schemas schc ON tc.schema_id = schc.schema_id
JOIN sys.tables tp ON fk.referenced_object_id = tp.object_id
JOIN sys.schemas shp ON tp.schema_id = shp.schema_id
WHERE tp.name = 'Stack' AND shp.name = 'dbo';

OPEN fk_cursor;
FETCH NEXT FROM fk_cursor INTO @fkId, @fkName, @childSchema, @childTable, @parentSchema, @parentTable;

WHILE @@FETCH_STATUS = 0
BEGIN
	-- Build comma-separated quoted column lists for child (parent_object) and parent (referenced_object)
	SELECT @childCols = STUFF((
		SELECT ',' + QUOTENAME(pc.name)
		FROM sys.foreign_key_columns fkc
		JOIN sys.columns pc ON fkc.parent_object_id = pc.object_id AND fkc.parent_column_id = pc.column_id
		WHERE fkc.constraint_object_id = @fkId
		ORDER BY fkc.constraint_column_id
		FOR XML PATH(''), TYPE).value('.','NVARCHAR(MAX)'),1,1,'');

	SELECT @parentCols = STUFF((
		SELECT ',' + QUOTENAME(rc.name)
		FROM sys.foreign_key_columns fkc
		JOIN sys.columns rc ON fkc.referenced_object_id = rc.object_id AND fkc.referenced_column_id = rc.column_id
		WHERE fkc.constraint_object_id = @fkId
		ORDER BY fkc.constraint_column_id
		FOR XML PATH(''), TYPE).value('.','NVARCHAR(MAX)'),1,1,'');

	SET @sql = N'PRINT N''Recreating constraint ' + QUOTENAME(@fkName) + N' on ' + QUOTENAME(@childSchema) + N'.' + QUOTENAME(@childTable) + N''';' + CHAR(13) +
			   N'ALTER TABLE ' + QUOTENAME(@childSchema) + N'.' + QUOTENAME(@childTable) + N' DROP CONSTRAINT ' + QUOTENAME(@fkName) + N';' + CHAR(13) +
			   N'ALTER TABLE ' + QUOTENAME(@childSchema) + N'.' + QUOTENAME(@childTable) + N' ADD CONSTRAINT ' + QUOTENAME(@fkName) + N' FOREIGN KEY (' + @childCols + N') REFERENCES ' + QUOTENAME(@parentSchema) + N'.' + QUOTENAME(@parentTable) + N'(' + @parentCols + N') ON DELETE CASCADE;';

	EXEC sp_executesql @sql;

	FETCH NEXT FROM fk_cursor INTO @fkId, @fkName, @childSchema, @childTable, @parentSchema, @parentTable;
END

CLOSE fk_cursor;
DEALLOCATE fk_cursor;

PRINT 'Completed updating foreign keys referencing dbo.Stack to ON DELETE CASCADE.';
GO
