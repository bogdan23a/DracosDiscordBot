using java.io;
using java.lang;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace com.sedmelluq.discord.lavaplayer.tools
{
    using Severity = com.sedmelluq.discord.lavaplayer.tools.FriendlyException.Severity;
    //using Logger = org.slf4j.Logger;


    /// <summary>
    /// Contains common helper methods for dealing with exceptions.
    /// </summary>
    public class ExceptionTools
    {
        /// <summary>
        /// Sometimes it is necessary to catch Throwable instances for logging or reporting purposes. However, unless for
        /// specific known cases, Error instances should not be blocked from propagating, so rethrow them.
        /// </summary>
        /// <param name="throwable"> The Throwable to check, it is rethrown if it is an Error </param>
        public static void rethrowErrors(System.Exception throwable)
        {
            if (throwable is System.Exception)
            {
                throw (System.Exception)throwable;
            }
        }

        /// <summary>
        /// If the exception is not a FriendlyException, wrap with a FriendlyException with the given message
        /// </summary>
        /// <param name="message"> Message of the new FriendlyException if needed </param>
        /// <param name="severity"> Severity of the new FriendlyException </param>
        /// <param name="throwable"> The exception to potentially wrap </param>
        /// <returns> Original or wrapped exception </returns>
        public static FriendlyException wrapUnfriendlyExceptions(string message, Severity severity, System.Exception throwable)
        {
            if (throwable is FriendlyException)
            {
                return (FriendlyException)throwable;
            }
            else
            {
                return new FriendlyException(message, severity, throwable);
            }
        }

        /// <summary>
        /// If the exception is not a FriendlyException, wrap with a RuntimeException
        /// </summary>
        /// <param name="throwable"> The exception to potentially wrap </param>
        /// <returns> Original or wrapped exception </returns>
        public static System.Exception wrapUnfriendlyExceptions(System.Exception throwable)
        {
            if (throwable is FriendlyException)
            {
                return (FriendlyException)throwable;
            }
            else
            {
                return new System.Exception(throwable.ToString());
            }
        }

        public static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            Type baseType = givenType.BaseType;
            if (baseType == null) return false;

            return IsAssignableToGenericType(baseType, genericType);
        }



        /// <summary>
        /// Finds the first exception which is an instance of the specified class from the throwable cause chain.
        /// </summary>
        /// <param name="throwable"> Throwable to scan. </param>
        /// <param name="klass"> The throwable class to scan for. </param>
        /// @param <T> The throwable class to scan for. </param>
        /// <returns> The first exception in the cause chain (including itself) which is an instance of the specified class. </returns>
        public static System.Type findDeepException<T>(System.Exception throwable, System.Type klass) where T : System.Exception
        {
            while (throwable != null)
            {
               
                if (IsAssignableToGenericType(klass, throwable.GetType()))
                {
                    return throwable.GetType();
                }

                throwable = throwable.InnerException;
            }

            return default(System.Type);
        }

        /// <summary>
        /// Makes sure thread is set to interrupted state when the throwable is an InterruptedException </summary>
        /// <param name="throwable"> Throwable to check </param>
        public static void keepInterrupted(System.Exception throwable)
        {
            if (throwable is InterruptedException)
            {
                System.Threading.Thread.CurrentThread.Interrupt();
            }
        }

        /// <summary>
        /// Log a FriendlyException appropriately according to its severity. </summary>
        /// <param name="log"> Logger instance to log it to </param>
        /// <param name="exception"> The exception itself </param>
        /// <param name="context"> An object that is included in the log </param>


        /// <summary>
        /// Encode an exception to an output stream </summary>
        /// <param name="output"> Data output </param>
        /// <param name="exception"> Exception to encode </param>
        /// <exception cref="IOException"> On IO error </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public static void encodeException(DataOutput output, FriendlyException exception) throws IOException
        public static void encodeException(DataOutput output, FriendlyException exception)
        {
            IList<System.Exception> causes = new List<System.Exception>();
            System.Exception next = exception.InnerException;

            while (next != null)
            {
                causes.Add(next);
                next = next.InnerException;
            }

            for (int i = causes.Count - 1; i >= 0; i--)
            {
                System.Exception cause = causes[i];
                output.writeBoolean(true);

                string message;

                if (cause is DecodedException)
                {
                    output.writeUTF(((DecodedException)cause).className);
                    message = ((DecodedException)cause).originalMessage;
                }
                else
                {
                    //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
                    output.writeUTF(cause.GetType().FullName);
                    message = cause.Message;
                }

                output.writeBoolean(!string.ReferenceEquals(message, null));
                if (!string.ReferenceEquals(message, null))
                {
                    output.writeUTF(message);
                }

                encodeStackTrace(output, cause);
            }

            output.writeBoolean(false);
            output.writeUTF(exception.Message);
            output.writeInt((int)exception.severity);

            decodeStackTrace(output, exception);
            encodeException
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: private static void encodeStackTrace(DataOutput output, Throwable throwable) throws IOException
        /*private static void encodeStackTrace(DataOutput output, System.Exception throwable)
        {
            StackTrace[] trace = throwable.StackTrace;
            output.writeInt(trace.Length);

            foreach (StackTrace element in trace)
            {
                output.writeUTF(element.getClassName());
                output.writeUTF(element.GetFrames().ToString());

                string fileName = element.getFileName();
                output.writeBoolean(!string.ReferenceEquals(fileName, null));
                if (!string.ReferenceEquals(fileName, null))
                {
                    output.writeUTF(fileName);
                }
                output.writeInt(element.getLineNumber());
            }
        }*/

        /// <summary>
        /// Decode an exception from an input stream </summary>
        /// <param name="input"> Data input </param>
        /// <returns> Decoded exception </returns>
        /// <exception cref="IOException"> On IO error </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public static FriendlyException decodeException(DataInput input) throws IOException
        public static FriendlyException decodeException(DataInput input)
        {
            DecodedException cause = null;

            while (input.readBoolean())
            {
                cause = new DecodedException(input.readUTF(), input.readBoolean() ? input.readUTF() : null, cause);
                cause.StackTrace = decodeStackTrace(input);
            }

            FriendlyException exception = new FriendlyException(input.readUTF(), typeof(Severity).EnumConstants[input.readInt()], cause);
            exception.StackTrace = decodeStackTrace(input);
            return exception;
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: private static StackTraceElement[] decodeStackTrace(DataInput input) throws IOException
        private static StackTraceElement[] decodeStackTrace(DataInput input)
        {
            StackTraceElement[] trace = new StackTraceElement[input.readInt()];

            for (int i = 0; i < trace.Length; i++)
            {
                trace[i] = new StackTraceElement(input.readUTF(), input.readUTF(), input.readBoolean() ? input.readUTF() : null, input.readInt());
            }

            return trace;
        }
    }
}