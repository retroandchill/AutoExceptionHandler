using System.Linq;
using ExceptionHandlerAnnotations;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;
using static ExceptionHandlerGenerator.Tests.Utils.GeneratorTestHelpers;

namespace ExceptionHandlerGenerator.Tests;

public class ExceptionHandlerGeneratorTest {
  private const string VectorClassText = """
                                         using System;
                                         using ExceptionHandlerAnnotations;

                                         namespace ExceptionHandlerGenerator.Sample;

                                         // This code will not compile until you build the project with the Source Generators

                                         [ExceptionHandler]
                                         public partial class ExampleHandler {
                                           [GeneralExceptionHandler]
                                           public partial int HandleException(Exception ex);
                                                                                      
                                           [HandlesException]
                                           public int HandleSingle(ArgumentNullException ex) {
                                             return 4;
                                           }
                                                                                        
                                           [HandlesException(typeof(NullReferenceException), typeof(ArithmeticException))]
                                           public int HandleMultiple(Exception ex) {
                                             return 5;
                                           }
                                           
                                           [FallbackExceptionHandler]
                                           public int HandleFallback(Exception ex) {
                                             return 6;
                                           }
                                         }
                                         """;

  private const string ExpectedGeneratedClassText = """
                                                    using System;
                                                    
                                                    namespace ExceptionHandlerGenerator.Sample;
                                                    
                                                    partial class ExampleHandler {
                                                    
                                                      public partial int HandleException(
                                                          System.Exception ex    
                                                          ) {
                                                        return ex switch {
                                                          System.ArgumentNullException e0 =>
                                                                HandleSingle(e0),
                                                          System.NullReferenceException or System.ArithmeticException =>
                                                              HandleMultiple((System.Exception) ex),
                                                        _ =>
                                                          HandleFallback(ex)
                                                        };
                                                      }
                                                      
                                                    }
                                                    """;

  [Fact]
  public void GenerateReportMethod() {
    // Create an instance of the source generator.
    var generator = new Generators.ExceptionHandlerGenerator();

    // Source generators should be tested using 'GeneratorDriver'.
    var driver = CSharpGeneratorDriver.Create(generator);

    // We need to create a compilation with the required source code.
    var compilation = CreateCompilation(VectorClassText, typeof(FallbackExceptionHandlerAttribute));

    // Run generators and retrieve all results.
    var runResult = driver.RunGenerators(compilation).GetRunResult();

    // All generated files can be found in 'RunResults.GeneratedTrees'.
    var generatedFileSyntax = runResult.GeneratedTrees.Single(t => t.FilePath.EndsWith("ExampleHandler.g.cs"));

    // Complex generators should be tested using text comparison.
    Assert.Equal(ExpectedGeneratedClassText, generatedFileSyntax.GetText().ToString(),
        ignoreLineEndingDifferences: true);
  }
}