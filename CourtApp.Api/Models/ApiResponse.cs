using System;
using System.Collections.Generic;

namespace CourtApp.Api.Models
{
    /// <summary>
    /// Standard API Response wrapper for all endpoints
    /// </summary>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Response status: true for success, false for failure
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// Response message describing the result
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Response data payload
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// HTTP Status Code
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Timestamp of the response
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Error details if any
        /// </summary>
        public List<string> Errors { get; set; }

        /// <summary>
        /// Constructor for success response
        /// </summary>
        public ApiResponse(bool status, string message, T data, int statusCode = 200, List<string> errors = null)
        {
            Status = status;
            Message = message;
            Data = data;
            StatusCode = statusCode;
            Errors = errors ?? new List<string>();
            Timestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Success response factory
        /// </summary>
        public static ApiResponse<T> Success(T data, string message = "Success", int statusCode = 200)
        {
            return new ApiResponse<T>(true, message, data, statusCode);
        }

        /// <summary>
        /// Failure response factory
        /// </summary>
        public static ApiResponse<T> Failure(string message, int statusCode = 400, List<string> errors = null)
        {
            return new ApiResponse<T>(false, message, default, statusCode, errors);
        }

        /// <summary>
        /// Error response factory
        /// </summary>
        public static ApiResponse<T> Error(string message, int statusCode = 500, List<string> errors = null)
        {
            return new ApiResponse<T>(false, message, default, statusCode, errors);
        }
    }

    /// <summary>
    /// Non-generic version for responses without data
    /// </summary>
    public class ApiResponse
    {
        /// <summary>
        /// Response status: true for success, false for failure
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// Response message describing the result
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// HTTP Status Code
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Timestamp of the response
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Error details if any
        /// </summary>
        public List<string> Errors { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ApiResponse(bool status, string message, int statusCode = 200, List<string> errors = null)
        {
            Status = status;
            Message = message;
            StatusCode = statusCode;
            Errors = errors ?? new List<string>();
            Timestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Success response factory
        /// </summary>
        public static ApiResponse Success(string message = "Success", int statusCode = 200)
        {
            return new ApiResponse(true, message, statusCode);
        }

        /// <summary>
        /// Failure response factory
        /// </summary>
        public static ApiResponse Failure(string message, int statusCode = 400, List<string> errors = null)
        {
            return new ApiResponse(false, message, statusCode, errors);
        }

        /// <summary>
        /// Error response factory
        /// </summary>
        public static ApiResponse Error(string message, int statusCode = 500, List<string> errors = null)
        {
            return new ApiResponse(false, message, statusCode, errors);
        }
    }
}
