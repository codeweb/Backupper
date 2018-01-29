using System;
using System.Collections.Generic;
using System.Text;

namespace Base.Configuration
{
  /// <summary>
  /// Gestione dell'esito di invio mail
  /// </summary>
  public class EmailSendResult
  {
    public bool IsOk;
    public Exception SendError;

    public static EmailSendResult CreateOk()
    {
      EmailSendResult result = new EmailSendResult();
      result.IsOk = true;

      return result;
    }

    public static EmailSendResult CreateError(Exception error)
    {
      EmailSendResult result = new EmailSendResult();
      result.IsOk = false;
      result.SendError = error;
      return result;
    }
  }
}
