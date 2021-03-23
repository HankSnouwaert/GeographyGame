using System;

namespace WPM
{
    public interface IErrorHandler
    {
        /// <summary>
        ///  Used to determine response to a given error
        /// </summary>
        ErrorState ErrorState { get; set; }

        /// <summary>
        ///  Report anticipated errors 
        /// </summary>
        /// <param name="message"></param> The error message for the user>
        /// <param name="state"<>/param> The state the error handler should be set to>
        /// <returns></returns> 
        void reportError(string message, ErrorState state);

        /// <summary>
        ///  Report an unanticipated exception
        /// </summary>
        /// <param name="ex"></param> The exception to report to the user>
        /// <returns></returns> 
        void catchException(Exception ex);

        /// <summary>
        ///  Execute the error responce determined by the error handler's error state
        /// </summary>
        void errorResponse();

    }
}