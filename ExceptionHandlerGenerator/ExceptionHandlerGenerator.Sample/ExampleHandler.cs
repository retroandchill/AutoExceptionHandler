
using System;
using ExceptionHandlerAnnotations;

namespace ExceptionHandlerGenerator.Sample;

// This code will not compile until you build the project with the Source Generators

[ExceptionHandler]
public partial class ExampleHandler {
  [GeneralExceptionHandler]
  public partial int HandleException(Exception ex);
                                             
  [HandlesException]
  private static int HandleSingle(ArgumentNullException ex) {
    return 4;
  }
                                               
  [HandlesException(typeof(NullReferenceException), typeof(ArithmeticException))]
  private static int HandleMultiple(Exception ex) {
    return 5;
  }
  
  [FallbackExceptionHandler]
  private static int HandleFallback(Exception ex) {
    return 6;
  }
}