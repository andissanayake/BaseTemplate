import { Button, Result } from "antd";
import React from "react";

type ErrorBoundaryProps = {
  fallback?: React.ReactNode;
  fallbackRender?: (error: Error, reset: () => void) => React.ReactNode;
  onError?: (error: Error, info: React.ErrorInfo) => void;
  resetKeys?: React.DependencyList;
  children: React.ReactNode;
};

type ErrorBoundaryState = {
  error: Error | null;
};

export class ErrorBoundary extends React.Component<
  ErrorBoundaryProps,
  ErrorBoundaryState
> {
  state: ErrorBoundaryState = { error: null };

  static getDerivedStateFromError(error: Error): ErrorBoundaryState {
    return { error };
  }

  componentDidCatch(error: Error, info: React.ErrorInfo) {
    this.props.onError?.(error, info);
  }

  componentDidUpdate(prevProps: ErrorBoundaryProps) {
    if (
      this.state.error &&
      this.props.resetKeys &&
      prevProps.resetKeys &&
      this.props.resetKeys.length === prevProps.resetKeys.length &&
      this.props.resetKeys.some((k, i) => k !== prevProps.resetKeys![i])
    ) {
      this.reset();
    }
  }

  reset = () => {
    this.setState({ error: null });
  };

  render() {
    const { error } = this.state;

    const { children, fallback, fallbackRender } = this.props;

    if (error) {
      if (fallbackRender) return <>{fallbackRender(error, this.reset)}</>;
      if (fallback) return <>{fallback}</>;
      return (
        <Result
          status="error"
          title="Something went wrong."
          subTitle={error.message}
          extra={[
            <Button
              key="home"
              type="primary"
              onClick={() => {
                this.reset();
                window.location.href = "/";
              }}
            >
              Go to homepage
            </Button>,
          ]}
        />
      );
    }

    return children as React.ReactElement;
  }
}
