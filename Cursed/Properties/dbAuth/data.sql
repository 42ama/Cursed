IF(NOT EXISTS(SELECT 1 FROM dbo.UserAuth))
BEGIN
INSERT INTO UserAuth
    (Login, PasswordHash)
VALUES 
    ('admin', 'gs0NkkMo0otSoUAJGhcocUtbyWj1llgcgtIv9ue+tRgglVIf'), 
    ('technolog', 'SbIGkFnZYXtX+IdbK088fIIA3C6dk0jH7Kuv4mIMMFtLscDe'), 
    ('seniorTechnolog', 'qTB5Bel0zY5rNRnXMHQFPRTjDy2atu6VcBpZ+kUiiGdXw8MB'),
    ('govermentAgent', '9zEovSgcb+0MdSGDRuNbB26FZOuI5ZzXDwsgwdlcoxc58PsJ'), 
    ('manager', 'iZSjGmQIIBQnCKiQft3w3mFmtyWBRgRKkIfXGttpOG35eQdq')
;
END;


IF(NOT EXISTS(SELECT 1 FROM dbo.Role))
BEGIN
INSERT INTO Role
    (Name)
VALUES 
    ('admin'),
    ('technologist'),
    ('seniorTechnologist'),
    ('govermentAgent'),
    ('manager')
;
END;


IF(NOT EXISTS(SELECT 1 FROM dbo.UserData))
BEGIN
INSERT INTO UserData
    (Login, RoleName)
VALUES 
    ('admin', 'admin'),
    ('technolog', 'technologist'),
    ('seniorTechnolog', 'seniorTechnologist'),
    ('govermentAgent', 'govermentAgent'),
    ('manager', 'manager')
;
END;