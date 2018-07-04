pipeline {
  agent {
    label 'Git'
  }
  stages {
    stage('Build') {
      steps {
	      powershell 'Invoke-Expression "& `"${env:EXECUTABLE_DOTNET_2_0}`" build -c Release"'
      }
    }
	stage('Test') {
		steps {
			powershell 'Invoke-Expression "& `"${env:EXECUTABLE_DOTNET_2_0}`" vstest --logger:trx ((ls -Recurse *.UnitTests.dll | % FullName) -Match `"\\\\bin\\\\Release\\\\`")"'
			mstest testResultsFile:"**/*.trx", keepLongStdio: true
		}
	}
	stage('Pack') {
		steps {
			powershell 'Invoke-Expression "& `"${env:EXECUTABLE_DOTNET_2_0}`" pack -c Release"'
		}
	}
	stage('Publish') {
		steps {
			echo "Publish"
		}
	}
  }
}
