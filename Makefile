# Makefile

# Set the C# compiler to use
CSC = dotnet

# Set the output directory for the compiled files
OUTPUT_DIR = ./bin


all: Program

Program: Program.cs
	$(CSC) build $(BUILD_FLAGS) NLPproject.csproj


clean:
	rm -rf $(OUTPUT_DIR)