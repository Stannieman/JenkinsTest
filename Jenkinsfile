pipeline {
  agent {
    label 'Git'
  }
  stages {
    stage('Build') {
      steps {
	      powershell 'Invoke-Expression "& `"${env:EXECUTABLE_MSBUILD_15_7}`" /m /t:restore /p:Configuration=Release"'
		powershell 'Invoke-Expression "& `"${env:EXECUTABLE_MSBUILD_15_7}`" /m /t:build /p:Configuration=Release"'
      }
    }
	stage('Test') {
		steps {
			powershell 'Write-Host "Teeest"'
		}
	}
	stage('Pack') {
		steps {
			powershell 'Invoke-Expression "& `"${env:EXECUTABLE_MSBUILD_15_7}`" /m /t:pack /p:Configuration=Release"'
		}
	}
	stage('Publish') {
		steps {
			echo "Publish"
		}
	}
  }
}
