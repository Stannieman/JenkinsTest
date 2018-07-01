pipeline {
  agent {
    label 'testlabel'
  }
  stages {
    stage('Build') {
      steps {
        powershell "mkdir testdir"
        cleanWS()
      }
    }
  }
}
