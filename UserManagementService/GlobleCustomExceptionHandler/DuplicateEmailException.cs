﻿namespace UserManagementService.GlobleCustomExceptionHandler
{
    public class DuplicateEmailException : Exception
    {
        public DuplicateEmailException(string message) : base(message) { }

    }   
}
