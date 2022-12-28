import CodeBlock from '@theme-original/CodeBlock';
import React from 'react';

export default function CodeBlockWrapper(props) {
  return (
    <>
      <CodeBlock {...props} showLineNumbers />
    </>
  );
}